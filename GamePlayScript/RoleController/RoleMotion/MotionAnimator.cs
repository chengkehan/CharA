using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;
using HomebrewIK;
using GameScript.Cutscene;

namespace GameScript
{
    public class MotionAnimator
    {
        public enum State
        {
            Undefined = 0,
            Idle = 1,
            Moving = 2,
            Turning = 4,
            Climbing = 8,
            JumpDown = 16,
            JumpUp = 32,
            Ladder = 64,
            DirectionLeft = 128, // To indicate turning direction, use to modify Turning.
            DirectionRight = 256, // To indicate turning direction, use to modify Turning.
            Fast = 512, // To indicate Moveing fast(Run) or slow(Walk), use to modify Moving.
            JumpForward = 1024, // To indicate jump direction, used to modify JumpUp.
            NoOperation = 2048, // To indicate ignore this operation, used to modify Idle.
            IsStair = 4096,  // To indicate walking up and down stair
            OpenDoor = 8192,
            Skill = 16384,
            Solo = 32768
        }

        private int _previousState = (int)State.Idle;
        private int _state = (int)State.Idle;
        private SkillSM.Transition pendingSkill = SkillSM.Transition.Undefined;
        private SoloSM.Transition pendingSolo = SoloSM.Transition.Undefined;
        private SoloSM.Transition recentSolo = SoloSM.Transition.Undefined;

        private Animator[] animators = null;

        private Dictionary<State, List<ActionStateMachine>> actionSMs = null;

        private MotionCorrection[] motionCorrections = null;

        private csHomebrewIK[] allFootIKs = null;

        private RoleHandIK[] allHandIKs = null;

        private RoleAnimation _roleAnimation = null;

        private bool _isMarkedAsStopMoving = false;

        // the last locked door role try to pass it
        private Door _lastLockedDoor = null;
        private Door lastLockedDoor
        {
            set
            {
                _lastLockedDoor = value;
            }
            get
            {
                return _lastLockedDoor;
            }
        }

        private const float HIGH_PLATFORM_VERTICAL_DROP = 1.6f;
        private const float VERY_HIGH_PLATFORM_VERTICAL_DROP = 2f;

        public MotionAnimator(RoleAnimation roleAnimation)
        {
            _roleAnimation = roleAnimation;
            Initialize(_roleAnimation.gameObject);
        }

        public void Update()
        {
            if (ContainsState(State.Turning) || ContainsState(State.Climbing) ||
                ContainsState(State.JumpDown) || ContainsState(State.JumpUp) ||
                ContainsState(State.OpenDoor))
            {
                return;
            }

            if (LadderUpdate())
            {
                return;
            }

            if (GetRoleAnimation().GetOperation().HasOperation())
            {
                // We can't do any operation when role is ready to port to another waypoint
                // or role ported from a waypoint just now.
                Waypoint targetWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();
                Waypoint previousTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetThePreviousWaypointInOriginalSearchingResult(targetWaypoint);
                if ((previousTargetWaypoint != null && previousTargetWaypoint.IsPortal()) ||
                    (targetWaypoint != null && targetWaypoint.IsPortal()))
                {
                    // Do nothing
                }
                else
                {
                    if (GetRoleAnimation().GetOperation().GetOperation().stopMovingBeforeExecute)
                    {
                        SetMarkAsStopMoving(true);
                    }
                    if (GetRoleAnimation().GetOperation().ExecuteOperation())
                    {
                        GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(null);
                    }
                    GetRoleAnimation().GetOperation().ClearOperation();

                    NewOperationEvent();

                    if (GetRoleAnimation().GetWaypointPath().ContainsDoorInSearchingResult(lastLockedDoor))
                    {
                        SetOpenDoorState();
                        return;
                    }
                    else
                    {
                        lastLockedDoor = null;
                    }
                }
            }

            GetRoleAnimation().GetOperation().OnSecondOperationUpdate();

            if (GetMarkAsStopMoving())
            {
                SetMarkAsStopMoving(false);
                GetRoleAnimation().GetWaypointPath().ClearSearchingResult();
                GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(null);
                SetIdleState(false);
            }

            if (GetRoleAnimation().GetWaypointPath().NumberWaypointsOfSearchingResult() > 0)
            {
                if (GetRoleAnimation().GetWaypointPath().GetTargetWaypoint() == null)
                {
                    if (GetRoleAnimation().GetWaypointPath().NumberWaypointsOfOriginalSearchingResult() <= 1)
                    {
                        var endWaypoint = GetRoleAnimation().GetWaypointPath().GetWaypointInOriginalSearchingResult(0);

                        NoWayToPointEvent();
                        SetIdleState(true);
                        GetRoleAnimation().GetWaypointPath().ClearSearchingResult();
                        MovingToWaypointsEndEvent(endWaypoint);
                        return;
                    }
                    else
                    {
                        GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult());
                        GetRoleAnimation().GetWaypointPath().CutFirstWaypointOfSearchingResult();
                    }
                }
            }

            if (GetRoleAnimation().GetWaypointPath().GetTargetWaypoint() != null)
            {
                if (GetRoleAnimation().GetWaypointPath().GetIsArrivedTargetWaypoint())
                {
                    if (GetRoleAnimation().GetWaypointPath().GetTargetWaypoint().type == Waypoint.Type.Climbing &&
                        GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult() != null &&
                        GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult().type == Waypoint.Type.JumpDown)
                    {
                        Vector3 referenceDirection = GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult().GetPosition();
                        referenceDirection.y = GetPosition().y;
                        referenceDirection = referenceDirection - GetPosition();
                        if (TryToSetTurningState(referenceDirection) == false)
                        {
                            GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(null);
                            SetState(State.Climbing);
                        }
                    }
                    else if (GetRoleAnimation().GetWaypointPath().GetTargetWaypoint().type == Waypoint.Type.JumpDown &&
                            GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult() != null &&
                            GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult().type == Waypoint.Type.Climbing)
                    {
                        GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(null);
                        SetState(State.JumpDown);
                    }
                    else if (GetRoleAnimation().GetWaypointPath().GetTargetWaypoint().type == Waypoint.Type.JumpUp)
                    {
                        GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(null);
                        SetState(State.JumpUp);
                    }
                    else
                    {
                        if (GetRoleAnimation().GetWaypointPath().GetTargetWaypoint().IsDoor())
                        {
                            SetOpenDoorState();
                        }
                        else
                        {
                            if (GetRoleAnimation().GetWaypointPath().GetTargetWaypoint().IsPortal())
                            {
                                var portalWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint().portToWaypoint;

                                GetRoleAnimation().GetWaypointPath().ClearSearchingResult();
                                GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(null);
                                SetIdleState(false);

                                GetRoleAnimation().GetOperation().ClearOperation();

                                SetPosition(portalWaypoint.GetPosition());
                                SetForward(portalWaypoint.GetForward());

                                Waypoint moveToWaypointAutomatically = null;
                                for (int neighbourI = 0; neighbourI < portalWaypoint.NumberNeighbours(); neighbourI++)
                                {
                                    if (portalWaypoint.GetNeighbour(neighbourI) != null)
                                    {
                                        moveToWaypointAutomatically = portalWaypoint.GetNeighbour(neighbourI);
                                        break;
                                    }
                                }
                                var movingOperation = GetRoleAnimation().GetOperation().GenerateMovingOperation(moveToWaypointAutomatically);
                                GetRoleAnimation().GetOperation().SetOperation(movingOperation);
                            }
                            else
                            {
                                if (GetRoleAnimation().GetWaypointPath().NumberWaypointsOfSearchingResult() < 1)
                                {
                                    var endWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();

                                    GetRoleAnimation().GetWaypointPath().ClearSearchingResult();
                                    GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(null);
                                    SetIdleState(false);
                                    MovingToWaypointsEndEvent(endWaypoint);
                                }
                                else
                                {
                                    GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(null);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Waypoint targetWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();
                    Waypoint previousTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetThePreviousWaypointInOriginalSearchingResult(targetWaypoint);
                    if (targetWaypoint != null && previousTargetWaypoint != null &&
                        (
                            targetWaypoint.type == Waypoint.Type.Ladder ||
                            (targetWaypoint.type != Waypoint.Type.Ladder && previousTargetWaypoint.type == Waypoint.Type.Ladder)
                        )
                       )
                    {
                        SetState(State.Ladder);
                    }
                    else
                    {
                        Vector3 targetDirection = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint().GetPosition() - GetPosition();
                        if (TryToSetTurningState(targetDirection) == false)
                        {
                            SetMovingState();
                        }
                    }
                }
            }
            else
            {
                if (ContainsState(State.Skill) || ContainsState(State.Solo))
                {
                    // Do nothing
                }
                else
                {
                    if (ContainsState(State.NoOperation))
                    {
                        SetIdleState(true);
                    }
                    else
                    {
                        SetIdleState(false);
                    }
                }
            }
        }

        private void StopShakeHeadEvent()
        {
            var notificationData = new StopShakeHeadND();
            notificationData.roleID = GetRoleAnimation().actor.o.GetId();
            EventSystem.GetInstance().Notify(EventID.StopShakeHead, notificationData);
        }

        private void NoWayToPointEvent()
        {
            var notificationData = new NoWayToPointND();
            notificationData.roleID = GetRoleAnimation().actor.o.GetId();
            EventSystem.GetInstance().Notify(EventID.NoWayToPoint, notificationData);
        }

        private void NewOperationEvent()
        {
            var notificationData = new NewOperationND();
            notificationData.roleID = GetRoleAnimation().actor.o.GetId();
            EventSystem.GetInstance().Notify(EventID.NewOperation, notificationData);
        }

        private void MovingToWaypointsEndEvent(Waypoint endWaypoint)
        {
            var notificationData = new MoveToWaypointsEndND();
            notificationData.roleID = GetRoleAnimation().actor.o.GetId();
            notificationData.endWaypoint = endWaypoint;
            EventSystem.GetInstance().Notify(EventID.MoveToWaypointsEnd, notificationData);
        }

        public RoleAnimation GetRoleAnimation()
        {
            return _roleAnimation;
        }

        public void SetMarkAsStopMoving(bool b)
        {
            _isMarkedAsStopMoving = b;
        }

        public bool GetMarkAsStopMoving()
        {
            return _isMarkedAsStopMoving;
        }

        #region Up body animation Layer2

        public enum UpBodyAnimationLayer2
        {
            None = 0,
            Headache = 1
        }

        private int _upBodyAnimationNameId2 = 0;
        private int upBodyAnimationNameId2
        {
            get
            {
                if (_upBodyAnimationNameId2 == 0)
                {
                    _upBodyAnimationNameId2 = Animator.StringToHash("UpBodyAnimation2");
                }
                return _upBodyAnimationNameId2;
            }
        }

        public void SetUpBodyAnimationLayer2(UpBodyAnimationLayer2 animation)
        {
            if (animators != null)
            {
                foreach (var animator in animators)
                {
                    animator.SetInteger(upBodyAnimationNameId2, (int)animation);
                }
            }
        }

        #endregion

        #region Up body animation

        public enum UpBodyAnimation
        {
            None = 0,
            StickInHands = 1
        }

        private int _upBodyAnimationNameId = 0;
        private int upBodyAnimationNameId
        {
            get
            {
                if (_upBodyAnimationNameId == 0)
                {
                    _upBodyAnimationNameId = Animator.StringToHash("UpBodyAnimation");
                }
                return _upBodyAnimationNameId;
            }
        }

        public void SetUpBodyAnimation(UpBodyAnimation animation)
        {
            if (animators != null)
            {
                foreach (var animator in animators)
                {
                    animator.SetInteger(upBodyAnimationNameId, (int)animation);
                }
            }
        }

        #endregion

        public void MatchTargetUpdate()
        {
            if (actionSMs != null && animators != null)
            {
                foreach (var asmList in actionSMs.Values)
                {
                    for (int asmI = 0; asmI < asmList.Count; asmI++)
                    {
                        if (animators.Length > asmI && animators[asmI] != null)
                        {
                            asmList[asmI].MatchTargetUpdate(animators[asmI]);
                        }
                    }
                }
            }
        }

        private void SetOpenDoorHandIKParams(bool handIK_isLeftHand)
        {
            if (motionCorrections != null)
            {
                foreach (var motionCorrection in motionCorrections)
                {
                    if (motionCorrection != null)
                    {
                        motionCorrection.SetOpenDoorHandIKParams(handIK_isLeftHand);
                    }
                }
            }
        }

        public bool AtRightHandSide(Vector3 referencePosition)
        {
            if (motionCorrections != null)
            {
                foreach (var motionCorrection in motionCorrections)
                {
                    if (motionCorrection != null)
                    {
                        return motionCorrection.AtRightHandSide(referencePosition);
                    }
                }
            }
            return true;
        }

        public bool AtLeftHandSide(Vector3 referencePosition)
        {
            if (motionCorrections != null)
            {
                foreach (var motionCorrection in motionCorrections)
                {
                    if (motionCorrection != null)
                    {
                        return motionCorrection.AtLeftHandSide(referencePosition);
                    }
                }
            }
            return true;
        }

        public bool IsLeftwardDirection(Vector3 referenceDirection)
        {
            if (motionCorrections != null)
            {
                foreach (var motionCorrection in motionCorrections)
                {
                    if (motionCorrection != null)
                    {
                        return motionCorrection.IsLeftwardDirection(referenceDirection);
                    }
                }
            }
            return true;
        }

        public bool IsRightwardDirection(Vector3 referenceDirection)
        {
            if (motionCorrections != null)
            {
                foreach (var motionCorrection in motionCorrections)
                {
                    if (motionCorrection != null)
                    {
                        return motionCorrection.IsRightwardDirection(referenceDirection);
                    }
                }
            }
            return true;
        }

        public bool IsBackwardDirection(Vector3 referenceDirection)
        {
            if (motionCorrections != null)
            {
                foreach (var motionCorrection in motionCorrections)
                {
                    if (motionCorrection != null)
                    {
                        return motionCorrection.IsBackwardDirection(referenceDirection);
                    }
                }
            }
            return true;
        }

        public MotionCorrection.MovingFoot GetMovingFoot()
        {
            if (motionCorrections != null)
            {
                foreach (var motionCorrection in motionCorrections)
                {
                    if (motionCorrection != null)
                    {
                        return motionCorrection.GetMovingFoot();
                    }
                }
            }
            return MotionCorrection.MovingFoot.Undefined;
        }

        public void SetForward(Vector3 forward)
        {
            foreach (var animator in animators)
            {
                if (animator != null && animator.transform != null)
                {
                    animator.transform.forward = forward;
                }
            }
        }

        public Vector3 GetForward()
        {
            if (animators.Length == 0 || animators[0] == null || animators[0].transform == null)
            {
                return Vector3.zero;
            }
            else
            {
                return animators[0].transform.forward;
            }
        }

        public Vector3 GetRight()
        {
            if (animators.Length == 0 || animators[0] == null || animators[0].transform == null)
            {
                return Vector3.zero;
            }
            else
            {
                return animators[0].transform.right;
            }
        }

        public void SetFootIK(bool enabled)
        {
            if (allFootIKs != null)
            {
                foreach (var footIK in allFootIKs)
                {
                    if (footIK != null)
                    {
                        footIK.enabled = enabled;
                    }
                }
            }
        }

        public void SetHandIK(bool enabled, Vector3 targetPosition, float weight, bool isLeftHand)
        {
            if (allHandIKs != null)
            {
                foreach (var handIK in allHandIKs)
                {
                    if (handIK != null)
                    {
                        handIK.enabled = enabled;
                        handIK.targetPosition = targetPosition;
                        handIK.weight = weight;
                        handIK.isLeftHand = isLeftHand;
                    }
                }
            }
        }

        public void SyncAnimators()
        {
            int mcI = -1;
            if (motionCorrections != null)
            {
                for (int i = 0; i < motionCorrections.Length; i++)
                {
                    if (motionCorrections[i] != null)
                    {
                        mcI = i;
                        break;
                    }
                }
            }
            if (animators != null && mcI >=0 && mcI < animators.Length)
            {
                var mcAnimator = animators[mcI];
                if (mcAnimator != null)
                {
                    var mcAnimT = mcAnimator.transform;
                    if (mcAnimT != null)
                    {
                        for (int i = 0; i < animators.Length; i++)
                        {
                            if (i != mcI)
                            {
                                var animator = animators[i];
                                if (animator != null)
                                {
                                    var animT = animator.transform;
                                    if (animT != null)
                                    {
                                        animT.position = mcAnimT.position;
                                        animT.eulerAngles = mcAnimT.eulerAngles;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetState(State state1, State state2 = State.Undefined)
        {
            _previousState = _state;
            _state = (int)state1;
            _state |= (int)state2;
            RefreshStateMachine();
        }

        public void SetSkillState(SkillSM.Transition skill)
        {
            pendingSkill = skill;
            SetState(State.Skill);
        }

        public void SetSoloState(SoloSM.Transition solo)
        {
            pendingSolo = solo;
            SetState(State.Solo);
        }

        public bool ContainsState(State state)
        {
            return (_state & (int)state) != 0;
        }

        private bool ContainsPreviousState(State state)
        {
            return (_previousState & (int)state) != 0;
        }

        public Vector3 GetPosition()
        {
            if (animators.Length == 0 || animators[0] == null || animators[0].transform == null)
            {
                return Vector3.zero;
            }
            else
            {
                return animators[0].transform.position;
            }
        }

        public void SetPosition(Vector3 position)
        {
            foreach (var animator in animators)
            {
                if (animator != null)
                {
                    animator.transform.position = position;
                }
            }
        }

        public Quaternion GetRotation()
        {
            if (animators.Length == 0 || animators[0] == null || animators[0].transform == null)
            {
                return Quaternion.identity;
            }
            else
            {
                return animators[0].transform.rotation;
            }
        }

        public void SetRotation(Quaternion rotation)
        {
            foreach (var animator in animators)
            {
                if (animator != null)
                {
                    animator.transform.rotation = rotation;
                }
            }
        }

        #region WalkRunSpeedScale

        private int _walkRunSpeedScaleID = -1;
        private int walkRunSpeedScale
        {
            get
            {
                if (_walkRunSpeedScaleID == -1)
                {
                    _walkRunSpeedScaleID = Animator.StringToHash("WalkRunSpeedScale");
                }
                return _walkRunSpeedScaleID;
            }
        }

        public void SetWalkRunSpeedScale(float scale)
        {
            if (animators != null)
            {
                foreach (var animator in animators)
                {
                    if (animator != null)
                    {
                        animator.SetFloat(walkRunSpeedScale, scale);
                    }
                }
            }
        }

        #endregion

        public void ExecuteCompleteCBManually(State state)
        {
            if (actionSMs != null && actionSMs.ContainsKey(state))
            {
                var list = actionSMs[state];
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        item.ExecuteActionCompleteCB();
                    }
                }
            }
        }

        public bool IsInTransition(State state)
        {
            if (actionSMs == null || actionSMs.ContainsKey(state) == false)
            {
                return false;
            }
            else
            {
                if (actionSMs[state] == null || actionSMs[state].Count == 0)
                {
                    return false;
                }
                else
                {
                    if (animators == null || animators.Length == 0 || animators[0] == null)
                    {
                        return false;
                    }
                    else
                    {
                        return actionSMs[state][0].IsInTransition(animators[0]);
                    }
                }
            }
        }

        private void SetActionValueAutomatically(State state, System.Enum value)
        {
            foreach (var stateObj in System.Enum.GetValues(typeof(State)))
            {
                var stateEnum = (State)stateObj;
                if (stateEnum == state)
                {
                    SetActionValue(state, value);
                }
                else
                {
                    SetActionValue(stateEnum, State.Undefined);
                }
            }
        }

        private void SetActionValue(State state, System.Enum value)
        {
            if (actionSMs != null && animators != null)
            {
                if (actionSMs.ContainsKey(state))
                {
                    var asmList = actionSMs[state];
                    for (int asmI = 0; asmI < asmList.Count; asmI++)
                    {
                        if (animators.Length > asmI && animators[asmI] != null)
                        {
                            asmList[asmI].SetAction(animators[asmI], System.Convert.ToInt32(value));
                        }
                    }
                }
            }
        }

        public void StopShakeHead(bool eventOnly)
        {
            if (ContainsState(State.Idle) && ContainsState(State.NoOperation))
            {
                StopShakeHeadEvent();
                if (eventOnly == false)
                {
                    SetIdleState(false);
                }
            }
        }

        private void SetIdleState(bool isNoOperation)
        {
            GetRoleAnimation().GetOperation().OnSecondOperationTheEnd();

            if (isNoOperation)
            {
                SetState(State.Idle, State.NoOperation);
            }
            else
            {
                SetState(State.Idle);
            }
        }

        private void SetOpenDoorState()
        {
            GetRoleAnimation().GetOperation().SetIsLocked(true);
            GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(null);
            SetState(State.OpenDoor);
        }

        private void SetMovingState()
        {
            if (GetRoleAnimation().GetWaypointPath().GetTargetWaypoint() != null &&
                GetRoleAnimation().GetWaypointPath().GetTargetWaypoint().isStair)
            {
                SetState(State.Moving, State.IsStair);
            }
            else
            {
                if (GetRoleAnimation().GetWaypointPath().NumberWaypointsOfOriginalSearchingResult() > 0)
                {
                    Vector3 rolePosition = GetPosition();
                    Waypoint pendingWaypoint = GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult();
                    Waypoint nextPendingWaypoint = GetRoleAnimation().GetWaypointPath().GetTheNextWaypointInOriginalSearchingResult(pendingWaypoint);
                    Waypoint targetWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();
                    Waypoint nextTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetTheNextWaypointInOriginalSearchingResult(targetWaypoint);
                    Waypoint previousTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetThePreviousWaypointInOriginalSearchingResult(targetWaypoint);

                    bool isCloseToTheLastWaypoint =
                        pendingWaypoint == null &&
                        targetWaypoint != null &&
                        Vector3.Distance(rolePosition, targetWaypoint.GetPosition()) < 1f;

                    bool willGotoLadder = nextTargetWaypoint != null && nextTargetWaypoint.type == Waypoint.Type.Ladder;

                    // role->--*p1---*p2---
                    // check p1 is climbing or jumpdown type and p2 also be climbing or jumpdown type.
                    bool willGotoClimbingEdge =
                        (targetWaypoint != null) &&
                        (
                            (targetWaypoint.type == Waypoint.Type.Climbing || targetWaypoint.type == Waypoint.Type.JumpDown) &&
                            (
                                (nextTargetWaypoint == null) ||
                                (
                                    (nextTargetWaypoint != null) &&
                                    (nextTargetWaypoint.type == Waypoint.Type.Climbing || nextTargetWaypoint.type == Waypoint.Type.JumpDown)
                                )
                            )
                        ) &&
                        (Vector3.Distance(rolePosition, targetWaypoint.GetPosition()) < 1f);

                    // the same as above
                    willGotoClimbingEdge =
                        willGotoClimbingEdge ||
                        (
                            (pendingWaypoint != null) &&
                            (
                                (pendingWaypoint.type == Waypoint.Type.Climbing || pendingWaypoint.type == Waypoint.Type.JumpDown) &&
                                (
                                    (nextPendingWaypoint == null) ||
                                    (
                                        (nextPendingWaypoint != null) &&
                                        (nextPendingWaypoint.type == Waypoint.Type.Climbing || nextPendingWaypoint.type == Waypoint.Type.JumpDown)
                                    )
                                )
                            ) &&
                            Vector3.Distance(rolePosition, pendingWaypoint.GetPosition()) < 1f
                        );

                    // On the dash path
                    //   p1*---p2*
                    //   |
                    //   |
                    //   p3*
                    //
                    // p3->p1
                    bool isAtTurningCorner = targetWaypoint != null && targetWaypoint.type == Waypoint.Type.Turning;
                    if (isAtTurningCorner &&
                        targetWaypoint != null && previousTargetWaypoint != null && nextTargetWaypoint != null &&
                        GetRoleAnimation().GetWaypointPath().IsApproximateTurning90(
                            targetWaypoint.GetPosition() - previousTargetWaypoint.GetPosition(),
                            nextTargetWaypoint.GetPosition() - targetWaypoint.GetPosition()
                        ))
                    {
                        isAtTurningCorner = true;
                    }
                    else
                    {
                        isAtTurningCorner = false;
                    }
                    if (isAtTurningCorner == false)
                    {
                        // p1 -> p2
                        isAtTurningCorner = previousTargetWaypoint != null && previousTargetWaypoint.type == Waypoint.Type.Turning;
                        Waypoint previousX2_TargetWaypoint = GetRoleAnimation().GetWaypointPath().GetThePreviousWaypointInOriginalSearchingResult(previousTargetWaypoint);
                        if (isAtTurningCorner &&
                            targetWaypoint != null && previousTargetWaypoint != null && previousX2_TargetWaypoint != null &&
                            GetRoleAnimation().GetWaypointPath().IsApproximateTurning90(
                                previousTargetWaypoint.GetPosition() - previousX2_TargetWaypoint.GetPosition(),
                                targetWaypoint.GetPosition() - previousTargetWaypoint.GetPosition()
                            ))
                        {
                            isAtTurningCorner = true;
                        }
                        else
                        {
                            isAtTurningCorner = false;
                        }
                    }

                    // Slow down when closed to door 
                    bool isClosedToDoor =
                        (targetWaypoint != null && targetWaypoint.IsDoor()) ||
                        (
                            nextTargetWaypoint != null && nextTargetWaypoint.IsDoor() &&
                            Vector3.Distance(rolePosition, nextTargetWaypoint.GetPosition()) < 1.5f
                        );

                    // speed up before arrived jumpup waypoint 
                    if (targetWaypoint != null && targetWaypoint.type == Waypoint.Type.JumpUp)
                    {
                        SetWalkRunSpeedScale(1.45f);
                    }
                    else
                    {
                        SetWalkRunSpeedScale(1);
                    }

                    if (isCloseToTheLastWaypoint || willGotoClimbingEdge || isAtTurningCorner || willGotoLadder || isClosedToDoor)
                    {
                        SetState(State.Moving);
                    }
                    else
                    {
                        SetState(State.Moving, State.Fast);
                    }
                }
                else
                {
                    Waypoint targetWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();
                    Waypoint previousTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetThePreviousWaypointInOriginalSearchingResult(targetWaypoint);
                    // Case1: When role port to tartet waypoint, he will move to next waypoint automatically.
                    // Case2: When role is moving to a portal.
                    // Both the two cases, we'd like to make role moving fastly.
                    if ((previousTargetWaypoint != null && previousTargetWaypoint.IsPortal()) ||
                        (targetWaypoint != null && targetWaypoint.IsPortal()))
                    {
                        SetState(State.Moving, State.Fast);
                    }
                    else
                    {
                        SetState(State.Moving);
                    }
                }
            }
        }

        private bool TryToSetTurningState(Vector3 targetDirection)
        {
            if (IsBackwardDirection(targetDirection))
            {
                SetState(State.Turning);
                return true;
            }
            else if (IsLeftwardDirection(targetDirection))
            {
                SetState(State.Turning, State.DirectionLeft);
                return true;
            }
            else if (IsRightwardDirection(targetDirection))
            {
                SetState(State.Turning, State.DirectionRight);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsWaypointsUpgrade()
        {
            var targetWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();
            if (targetWaypoint == null)
            {
                return false;
            }
            else
            {
                var rolePosition = GetPosition();
                return rolePosition.y < targetWaypoint.GetPosition().y;
            }
        }

        private bool IsWaypointsDowngrade()
        {
            var targetWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();
            if (targetWaypoint == null)
            {
                return false;
            }
            else
            {
                var rolePosition = GetPosition();
                return rolePosition.y > targetWaypoint.GetPosition().y;
            }
        }

        private bool WillClimbOnVeryHighPlatform()
        {
            float verticalDrop = GetVerticalDropForClimbingAndJumpDown();
            return verticalDrop < -VERY_HIGH_PLATFORM_VERTICAL_DROP;
        }

        // Check whether role will climb a high platform or a low plarform
        // return true: high platform
        // return false: low platform
        private bool WillClimbOnHighPlatform()
        {
            float verticalDrop = GetVerticalDropForClimbingAndJumpDown();
            return verticalDrop < -HIGH_PLATFORM_VERTICAL_DROP && verticalDrop > -VERY_HIGH_PLATFORM_VERTICAL_DROP;
        }

        private bool WillJumpDownFromVeryHighPlatform()
        {
            float verticalDrop = GetVerticalDropForClimbingAndJumpDown();
            return verticalDrop > VERY_HIGH_PLATFORM_VERTICAL_DROP;
        }

        // Check Whether role will jump down from a low platform
        // return true: high platform
        // return false: low platform
        private bool WillJumpDownFromHighPlatform()
        {
            float verticalDrop = GetVerticalDropForClimbingAndJumpDown();
            return verticalDrop > HIGH_PLATFORM_VERTICAL_DROP && verticalDrop < VERY_HIGH_PLATFORM_VERTICAL_DROP;
        }

        // Measure vertical drop before climbing and jumpdown
        private float GetVerticalDropForClimbingAndJumpDown()
        {
            Waypoint platformWaypoint = GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult();
            if (platformWaypoint != null)
            {
                Vector3 rolePosition = GetRoleAnimation().GetMotionAnimator().GetPosition();
                Vector3 targetPosition = platformWaypoint.GetPosition();
                return rolePosition.y - targetPosition.y;
            }
            else
            {
                return 0;
            }
        }

        // return true : output success
        // return false : output failure
        // output ladderUpOrLadderDown : is ladderUp
        // output ladderUpOrLadderDown : is ladderDown
        private bool IsLadderUpOrLadderDown(Waypoint targetWaypoint, Waypoint previousTargetWaypoint, out bool ladderUpOrLadderDown)
        {
            ladderUpOrLadderDown = false;

            if (targetWaypoint == null || previousTargetWaypoint == null)
            {
                return false;
            }

            if (targetWaypoint.type != Waypoint.Type.Ladder && previousTargetWaypoint.type != Waypoint.Type.Ladder)
            {
                return false;
            }

            ladderUpOrLadderDown = targetWaypoint.GetPosition().y > previousTargetWaypoint.GetPosition().y;

            return true;
        }
        private bool IsLadderUpOrLadderDown(out bool ladderUpOrLadderDown)
        {
            ladderUpOrLadderDown = false;

            if (GetLadderWaypoints(out Waypoint targetWaypoint, out Waypoint previousTargetWaypoint))
            {
                return IsLadderUpOrLadderDown(targetWaypoint, previousTargetWaypoint, out ladderUpOrLadderDown);
            }
            else
            {
                return false;
            }
        }
        private bool IsInLadderUpStart(Waypoint targetWaypoint, Waypoint previousTargetWaypoint)
        {
            return targetWaypoint != null && previousTargetWaypoint != null &&
                    previousTargetWaypoint.type != Waypoint.Type.Ladder &&
                    targetWaypoint.type == Waypoint.Type.Ladder &&
                    IsLadderUpOrLadderDown(targetWaypoint, previousTargetWaypoint, out bool ladderUpOrLadderDown) && ladderUpOrLadderDown;
        }
        public bool IsInLadderUpStart()
        {
            if (GetLadderWaypoints(out Waypoint targetWaypoint, out Waypoint previousTargetWaypoint))
            {
                return IsInLadderUpStart(targetWaypoint, previousTargetWaypoint);
            }
            else
            {
                return false;
            }
        }
        private bool IsInLadderUpClimbing(Waypoint targetWaypoint, Waypoint previousTargetWaypoint)
        {
            return targetWaypoint != null && previousTargetWaypoint != null &&
                    previousTargetWaypoint.type == Waypoint.Type.Ladder &&
                    targetWaypoint.type == Waypoint.Type.Ladder &&
                    IsLadderUpOrLadderDown(targetWaypoint, previousTargetWaypoint, out bool ladderUpOrLadderDown) && ladderUpOrLadderDown;
        }
        public bool IsInLadderUpClimbing()
        {
            if (GetLadderWaypoints(out Waypoint targetWaypoint, out Waypoint previousTargetWaypoint))
            {
                return IsInLadderUpClimbing(targetWaypoint, previousTargetWaypoint);
            }
            else
            {
                return false;
            }
        }
        private bool IsInLadderUpEnd(Waypoint targetWaypoint, Waypoint previousTargetWaypoint)
        {
            return targetWaypoint != null && previousTargetWaypoint != null &&
                    previousTargetWaypoint.type == Waypoint.Type.Ladder &&
                    targetWaypoint.type != Waypoint.Type.Ladder &&
                    IsLadderUpOrLadderDown(targetWaypoint, previousTargetWaypoint, out bool ladderUpOrLadderDown) && ladderUpOrLadderDown;
        }
        public bool IsInLadderUpEnd()
        {
            if (GetLadderWaypoints(out Waypoint targetWaypoint, out Waypoint previousTargetWaypoint))
            {
                return IsInLadderUpEnd(targetWaypoint, previousTargetWaypoint);
            }
            else
            {
                return false;
            }
        }
        private bool IsInLadderDownStart(Waypoint targetWaypoint, Waypoint previousTargetWaypoint)
        {
            return targetWaypoint != null && previousTargetWaypoint != null &&
                    targetWaypoint.type == Waypoint.Type.Ladder &&
                    previousTargetWaypoint.type != Waypoint.Type.Ladder &&
                    IsLadderUpOrLadderDown(targetWaypoint, previousTargetWaypoint, out bool ladderUpOrLadderDown) && !ladderUpOrLadderDown;
        }
        public bool IsInLadderDownStart()
        {
            if (GetLadderWaypoints(out Waypoint targetWaypoint, out Waypoint previousTargetWaypoint))
            {
                return IsInLadderDownStart(targetWaypoint, previousTargetWaypoint);
            }
            else
            {
                return false;
            }
        }
        private bool IsInLadderDownClimbing(Waypoint targetWaypoint, Waypoint previousTargetWaypoint)
        {
            return targetWaypoint != null && previousTargetWaypoint != null &&
                    targetWaypoint.type == Waypoint.Type.Ladder &&
                    previousTargetWaypoint.type == Waypoint.Type.Ladder &&
                    IsLadderUpOrLadderDown(targetWaypoint, previousTargetWaypoint, out bool ladderUpOrLadderDown) && !ladderUpOrLadderDown;
        }
        public bool IsInLadderDownClimbing()
        {
            if (GetLadderWaypoints(out Waypoint targetWaypoint, out Waypoint previousTargetWaypoint))
            {
                return IsInLadderDownClimbing(targetWaypoint, previousTargetWaypoint);
            }
            else
            {
                return false;
            }
        }
        private bool IsInLadderDownEnd(Waypoint targetWaypoint, Waypoint previousTargetWaypoint)
        {
            return targetWaypoint != null && previousTargetWaypoint != null &&
                    targetWaypoint.type != Waypoint.Type.Ladder &&
                    previousTargetWaypoint.type == Waypoint.Type.Ladder &&
                    IsLadderUpOrLadderDown(targetWaypoint, previousTargetWaypoint, out bool ladderUpOrLadderDown) && !ladderUpOrLadderDown;
        }
        public bool IsInLadderDownEnd()
        {
            if (GetLadderWaypoints(out Waypoint targetWaypoint, out Waypoint previousTargetWaypoint))
            {
                return IsInLadderDownEnd(targetWaypoint, previousTargetWaypoint);
            }
            else
            {
                return false;
            }
        }
        private bool GetLadderWaypoints(out Waypoint targetWaypoint, out Waypoint previousTargetWaypoint)
        {
            targetWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();
            previousTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetThePreviousWaypointInOriginalSearchingResult(targetWaypoint);
            return targetWaypoint != null && previousTargetWaypoint != null;
        }
        private bool LadderStartFacingDirection(bool upOrDown)
        {
            // Check facing direction and turning firstly
            Waypoint targetWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();
            Vector3 targetPosition = targetWaypoint.GetPosition();
            Vector3 rolePosition = GetPosition();
            Vector3 targetDirection = new Vector3(targetPosition.x, rolePosition.y, targetPosition.z) - rolePosition;
            if (upOrDown)
            {
                return TryToSetTurningState(targetDirection);
            }
            else
            {
                if (IsBackwardDirection(targetDirection) == false)
                {
                    SetState(State.Turning);
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        // Ladder is not a normal update loop.
        // It's a unique, do it specially.
        private bool LadderUpdate()
        {
            if (ContainsState(State.Ladder))
            {
                if (IsInLadderUpClimbing() || IsInLadderDownClimbing())
                {
                    if (GetLadderWaypoints(out Waypoint targetWaypoint, out Waypoint previousTargetWaypoint))
                    {
                        if (GetRoleAnimation().GetOperation().HasOperation())
                        {
                            // Generate two operations and compare the result of them.
                            // Choose the longer path searching result one.
                            var optionalOperation1 = GetRoleAnimation().GetOperation().GenerateLadderMotionOperation(
                                previousTargetWaypoint, GetRoleAnimation().GetOperation().GetOperation()
                            );
                            var optionalOperation2 = GetRoleAnimation().GetOperation().GenerateLadderMotionOperation(
                                targetWaypoint, GetRoleAnimation().GetOperation().GetOperation()
                            );

                            int numberWaypoints1 = -1;
                            GetRoleAnimation().GetOperation().SetOperation(optionalOperation1);
                            if (GetRoleAnimation().GetOperation().ExecuteOperation())
                            {
                                numberWaypoints1 = GetRoleAnimation().GetWaypointPath().NumberWaypointsOfSearchingResult();
                            }

                            int numberWaypoints2 = -1;
                            GetRoleAnimation().GetOperation().SetOperation(optionalOperation2);
                            if (GetRoleAnimation().GetOperation().ExecuteOperation())
                            {
                                numberWaypoints2 = GetRoleAnimation().GetWaypointPath().NumberWaypointsOfSearchingResult();
                            }

                            if (numberWaypoints1 != -1 && numberWaypoints2 != -1)
                            {
                                var operation = numberWaypoints1 > numberWaypoints2 ? optionalOperation1 : optionalOperation2;
                                GetRoleAnimation().GetOperation().SetOperation(operation);
                                if (GetRoleAnimation().GetOperation().ExecuteOperation())
                                {
                                    // Remove the first ladder type waypoint
                                    GetRoleAnimation().GetWaypointPath().CutFirstWaypointOfSearchingResult();

                                    Waypoint newTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult();
                                    GetRoleAnimation().GetWaypointPath().CutFirstWaypointOfSearchingResult();
                                    GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(newTargetWaypoint);

                                    SetState(State.Ladder);
                                }
                                GetRoleAnimation().GetOperation().ClearOperation();
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Initialize(GameObject go)
        {
            if (animators == null && go != null)
            {
                animators = go.GetComponentsInChildren<Animator>(false);

                actionSMs = new Dictionary<State, List<ActionStateMachine>>();
                actionSMs[State.Idle] = new List<ActionStateMachine>();
                actionSMs[State.Moving] = new List<ActionStateMachine>();
                actionSMs[State.Turning] = new List<ActionStateMachine>();
                actionSMs[State.Climbing] = new List<ActionStateMachine>();
                actionSMs[State.JumpDown] = new List<ActionStateMachine>();
                actionSMs[State.JumpUp] = new List<ActionStateMachine>();
                actionSMs[State.Ladder] = new List<ActionStateMachine>();
                actionSMs[State.OpenDoor] = new List<ActionStateMachine>();
                actionSMs[State.Skill] = new List<ActionStateMachine>();
                actionSMs[State.Solo] = new List<ActionStateMachine>();

                motionCorrections = new MotionCorrection[animators.Length];
                allFootIKs = new csHomebrewIK[animators.Length];
                allHandIKs = new RoleHandIK[animators.Length];
                for (int animatorI = 0; animatorI < animators.Length; animatorI++)
                {
                    actionSMs[State.Idle].Add(animators[animatorI].GetBehaviour<IdleSM>());
                    actionSMs[State.Moving].Add(animators[animatorI].GetBehaviour<MovingSM>());
                    actionSMs[State.Turning].Add(animators[animatorI].GetBehaviour<TurningSM>());
                    actionSMs[State.Climbing].Add(animators[animatorI].GetBehaviour<ClimbingSM>());
                    actionSMs[State.JumpDown].Add(animators[animatorI].GetBehaviour<JumpDownSM>());
                    actionSMs[State.JumpUp].Add(animators[animatorI].GetBehaviour<JumpUpSM>());
                    actionSMs[State.Ladder].Add(animators[animatorI].GetBehaviour<LadderSM>());
                    actionSMs[State.OpenDoor].Add(animators[animatorI].GetBehaviour<OpenDoorSM>());
                    actionSMs[State.Skill].Add(animators[animatorI].GetBehaviour<SkillSM>());
                    actionSMs[State.Solo].Add(animators[animatorI].GetBehaviour<SoloSM>());

                    motionCorrections[animatorI] = animators[animatorI].gameObject.GetComponent<MotionCorrection>();
                    if (motionCorrections[animatorI] != null)
                    {
                        motionCorrections[animatorI].SetAnimator(animators[animatorI]);
                        motionCorrections[animatorI].SetRoleAnimation(GetRoleAnimation());
                    }

                    allFootIKs[animatorI] = animators[animatorI].gameObject.GetComponent<csHomebrewIK>();
                    allHandIKs[animatorI] = animators[animatorI].gameObject.GetComponent<RoleHandIK>();
                }

                SetFootIK(false);
                SetHandIK(false, Vector3.zero, 0, false);

                foreach (var kv in actionSMs)
                {
                    var state = kv.Key;
                    var asmList = kv.Value;
                    for (int asmI = 0; asmI < asmList.Count; asmI++)
                    {
                        asmList[asmI].SetRoleAnimation(GetRoleAnimation());

                        // Only the first one will trigger OnActionCompleteCallback
                        if (asmI == 0)
                        {
                            var actionCompleteCB =
                                state == State.Turning ? new ActionStateMachine.ActionCompleteCB(TurningCompleteCB) :
                                state == State.Climbing ? new ActionStateMachine.ActionCompleteCB(ClimbingCompleteCB) :
                                state == State.JumpDown ? new ActionStateMachine.ActionCompleteCB(JumpDownCompleteCB) :
                                state == State.JumpUp ? new ActionStateMachine.ActionCompleteCB(JumpUpCompleteCB) :
                                state == State.Idle ? new ActionStateMachine.ActionCompleteCB(IdleCompleteCB) :
                                state == State.Ladder ? new ActionStateMachine.ActionCompleteCB(LadderCompleteCB) :
                                state == State.OpenDoor ? new ActionStateMachine.ActionCompleteCB(OpenDoorCompleteCB) :
                                state == State.Skill ? new ActionStateMachine.ActionCompleteCB(SkillCompleteCB) :
                                state == State.Solo ? new ActionStateMachine.ActionCompleteCB(SoloCompleteCB) :  new ActionStateMachine.ActionCompleteCB(DummyCompleteCB);
                            asmList[asmI].SetActionCompleteCB(actionCompleteCB);
                        }

                    }
                }
            }
        }

        private void RefreshStateMachine()
        {
            if (ContainsState(State.Idle))
            {
                if (ContainsState(State.NoOperation))
                {
                    SetActionValueAutomatically(State.Idle, IdleSM.Transition.ShakeHeadNoIdle);
                }
                else
                {
                    SetActionValueAutomatically(State.Idle, IdleSM.Transition.NormalIdle);
                }
            }
            else if (ContainsState(State.Moving) && !ContainsState(State.Fast))
            {
                if (ContainsState(State.IsStair))
                {
                    if (IsWaypointsUpgrade())
                    {
                        SetActionValueAutomatically(State.Moving, MovingSM.Transition.StairsAscending);
                    }
                    else
                    {
                        SetActionValueAutomatically(State.Moving, MovingSM.Transition.StairsDescending);
                    }
                }
                else
                {
                    SetActionValueAutomatically(State.Moving, MovingSM.Transition.Walk);
                }
            }
            else if (ContainsState(State.Moving) && ContainsState(State.Fast))
            {
                SetActionValueAutomatically(State.Moving, MovingSM.Transition.Run);
            }
            else if (ContainsState(State.Climbing))
            {
                if (WillClimbOnVeryHighPlatform())
                {
                    SetActionValueAutomatically(State.Climbing, ClimbingSM.Transition.ClimbingHighFreehandJumpHigh);
                }
                else if (WillClimbOnHighPlatform())
                {
                    SetActionValueAutomatically(State.Climbing, ClimbingSM.Transition.ClimbingNormal);
                }
                else
                {
                    SetActionValueAutomatically(State.Climbing, ClimbingSM.Transition.ClimbingLowNormal);
                }
            }
            else if (ContainsState(State.JumpDown))
            {
                if (WillJumpDownFromVeryHighPlatform())
                {
                    SetActionValueAutomatically(State.JumpDown, JumpDownSM.Transition.JumpDownHardLanding);
                }
                else if (WillJumpDownFromHighPlatform())
                {
                    SetActionValueAutomatically(State.JumpDown, JumpDownSM.Transition.JumpDownFarHigh);
                }
                else
                {
                    SetActionValueAutomatically(State.JumpDown, JumpDownSM.Transition.JumpDownNear);
                }
            }
            else if (ContainsState(State.Turning))
            {
                if (ContainsState(State.DirectionLeft))
                {
                    if (ContainsPreviousState(State.Moving) && !ContainsPreviousState(State.Fast))
                    {
                        //SetActionValueAutomatically(State.Turning, TurningSM.Transition.WalkTurnLeft90);
                        SetActionValueAutomatically(State.Turning, TurningSM.Transition.IdleTurnLeft90);
                    }
                    else if (ContainsPreviousState(State.Moving) && ContainsPreviousState(State.Fast))
                    {
                        SetActionValueAutomatically(State.Turning, TurningSM.Transition.WalkTurnLeft90);
                        //SetActionValueAutomatically(State.Turning, TurningSM.Transition.RunTurnLeft90);
                    }
                    else // ContainsPreviousState(State.Idle) or other cases
                    {
                        SetActionValueAutomatically(State.Turning, TurningSM.Transition.IdleTurnLeft90);
                    }
                }
                else if (ContainsState(State.DirectionRight))
                {
                    if (ContainsPreviousState(State.Moving) && !ContainsPreviousState(State.Fast))
                    {
                        //SetActionValueAutomatically(State.Turning, TurningSM.Transition.WalkTurnRight90);
                        SetActionValueAutomatically(State.Turning, TurningSM.Transition.IdleTurnRight90);
                    }
                    else if (ContainsPreviousState(State.Moving) && ContainsPreviousState(State.Fast))
                    {
                        SetActionValueAutomatically(State.Turning, TurningSM.Transition.WalkTurnRight90);
                        //SetActionValueAutomatically(State.Turning, TurningSM.Transition.RunTurnRight90);
                    }
                    else // ContainsPreviousState(State.Idle) or other cases
                    {
                        SetActionValueAutomatically(State.Turning, TurningSM.Transition.IdleTurnRight90);
                    }
                }
                else
                {
                    if (ContainsPreviousState(State.Moving) && !ContainsPreviousState(State.Fast))
                    {
                        if (Random.value < 0.5f)
                        {
                            //SetActionValueAutomatically(State.Turning, TurningSM.Transition.WalkTurnLeft);
                            SetActionValueAutomatically(State.Turning, TurningSM.Transition.IdleTurnLeft);
                        }
                        else
                        {
                            //SetActionValueAutomatically(State.Turning, TurningSM.Transition.WalkTurnRight);
                            SetActionValueAutomatically(State.Turning, TurningSM.Transition.IdleTurnRight);
                        }
                    }
                    else if (ContainsPreviousState(State.Moving) && ContainsPreviousState(State.Fast))
                    {
                        if (Random.value < 0.5f)
                        {
                            //SetActionValueAutomatically(State.Turning, TurningSM.Transition.RunTurnLeft);
                            SetActionValueAutomatically(State.Turning, TurningSM.Transition.WalkTurnLeft);
                        }
                        else
                        {
                            //SetActionValueAutomatically(State.Turning, TurningSM.Transition.RunTurnRight);
                            SetActionValueAutomatically(State.Turning, TurningSM.Transition.WalkTurnRight);
                        }
                    }
                    else // ContainsPreviousState(State.Idle) or other cases
                    {
                        if (Random.value < 0.5f)
                        {
                            SetActionValueAutomatically(State.Turning, TurningSM.Transition.IdleTurnLeft);
                        }
                        else
                        {
                            SetActionValueAutomatically(State.Turning, TurningSM.Transition.IdleTurnRight);
                        }
                    }
                }
            }
            else if (ContainsState(State.JumpUp))
            {
                if (GetMovingFoot() == MotionCorrection.MovingFoot.Left)
                {
                    SetActionValueAutomatically(State.JumpUp, JumpUpSM.Transition.JumpRunForward);
                }
                else
                {
                    SetActionValueAutomatically(State.JumpUp, JumpUpSM.Transition.JumpRunForwardMirrored);
                }

            }
            else if (ContainsState(State.Ladder))
            {
                if (IsInLadderUpStart())
                {
                    if (LadderStartFacingDirection(true) == false)
                    {
                        SetActionValueAutomatically(State.Ladder, LadderSM.Transition.LadderUpStart);
                    }
                }
                else if (IsInLadderDownStart())
                {
                    if (LadderStartFacingDirection(false) == false)
                    {
                        SetActionValueAutomatically(State.Ladder, LadderSM.Transition.LadderDownStart);
                    }
                }
                else if (IsInLadderUpClimbing())
                {
                    SetActionValueAutomatically(State.Ladder, LadderSM.Transition.LadderUpClimbing);
                }
                else if (IsInLadderDownClimbing())
                {
                    SetActionValueAutomatically(State.Ladder, LadderSM.Transition.LadderDownClimbing);
                }
                else if (IsInLadderUpEnd())
                {
                    SetActionValueAutomatically(State.Ladder, LadderSM.Transition.LadderUpEnd);
                }
                else if (IsInLadderDownEnd())
                {
                    SetActionValueAutomatically(State.Ladder, LadderSM.Transition.LadderDownEnd);
                }
                else
                {
                    // Do nothing
                }
            }
            else if (ContainsState(State.OpenDoor))
            {
                RefreshStateMachine_OpenDoor();
            }
            else if (ContainsState(State.Skill))
            {
                Utils.Assert(pendingSkill != SkillSM.Transition.Undefined);
                SetActionValueAutomatically(State.Skill, pendingSkill);
                pendingSkill = SkillSM.Transition.Undefined;
            }
            else if (ContainsState(State.Solo))
            {
                Utils.Assert(pendingSolo != SoloSM.Transition.Undefined);
                SetActionValueAutomatically(State.Solo, pendingSolo);
                recentSolo = pendingSolo;
                pendingSolo = SoloSM.Transition.Undefined;
            }
            else
            {
                // Do nothing
            }
        }

        private void RefreshStateMachine_OpenDoor()
        {
            if (lastLockedDoor == null)
            {
                var nextTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult();
                var prevTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetThePreviousWaypointInOriginalSearchingResult(nextTargetWaypoint);
                if (prevTargetWaypoint != null && prevTargetWaypoint.IsDoor())
                {
                    Vector3 rolePosition = GetPosition();
                    Vector3 roleRight = GetRight();

                    bool handIK_isLeftHand = prevTargetWaypoint.door.KnobAtLeftHandSide(roleRight, rolePosition);
                    if (prevTargetWaypoint.door.AtDoorBackSide(rolePosition))
                    {
                        handIK_isLeftHand = !handIK_isLeftHand;
                    }
                    SetOpenDoorHandIKParams(handIK_isLeftHand);

                    if (DataCenter.query.ActorHasKeyItem(GetRoleAnimation().actor.o.pd, prevTargetWaypoint.door.keyToDoorPairs))
                    {
                        prevTargetWaypoint.door.Open();

                        if (prevTargetWaypoint.door.AtDoorFrontSide(rolePosition))
                        {
                            if (prevTargetWaypoint.door.KnobAtRightHandSide(roleRight, rolePosition))
                            {
                                SetActionValueAutomatically(State.OpenDoor, OpenDoorSM.Transition.OpenDoorInwards);
                            }
                            else
                            {
                                SetActionValueAutomatically(State.OpenDoor, OpenDoorSM.Transition.OpenDoorInwardsMirror);
                            }
                        }
                        else
                        {
                            if (prevTargetWaypoint.door.KnobAtRightHandSide(roleRight, rolePosition))
                            {
                                SetActionValueAutomatically(State.OpenDoor, OpenDoorSM.Transition.OpenDoorOutwardsMirror);
                            }
                            else
                            {
                                SetActionValueAutomatically(State.OpenDoor, OpenDoorSM.Transition.OpenDoorOutwards);
                            }
                        }
                    }
                    else
                    {
                        lastLockedDoor = prevTargetWaypoint.door;
                        SetActionValueAutomatically(State.OpenDoor, OpenDoorSM.Transition.OpenDoorLocked);
                        SendWannaOpenLockedDoorEvent();
                    }
                }
                else
                {
                    Utils.LogObservably("Unexpected");
                }
            }
            else
            {
                SetActionValueAutomatically(State.OpenDoor, OpenDoorSM.Transition.OpenDoorLocked);
                SendWannaOpenLockedDoorEvent();
            }
        }

        private void SendWannaOpenLockedDoorEvent()
        {
            var notificationData = new WannaOpenLockedDoorND();
            notificationData.actorGUID = GetRoleAnimation().actor.o.guid;
            EventSystem.GetInstance().Notify(EventID.WannaOpenLockedDoor, notificationData);
        }

        private void LadderCompleteCB()
        {
            if (IsInLadderUpStart() ||
                IsInLadderUpClimbing() ||
                IsInLadderDownStart() ||
                IsInLadderDownClimbing())
            {
                Waypoint newTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult();
                GetRoleAnimation().GetWaypointPath().CutFirstWaypointOfSearchingResult();

                GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(newTargetWaypoint);
                SetState(State.Ladder);
            }
            else if (IsInLadderUpEnd() ||
                    IsInLadderDownEnd())
            {
                if (IsInLadderUpEnd())
                {
                    GetRoleAnimation().GetWaypointPath().CutOppisiteDirectionWaypointsOfSearchingResult(GetForward(), GetPosition());
                }

                Waypoint newTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult();
                GetRoleAnimation().GetWaypointPath().CutFirstWaypointOfSearchingResult();

                if (newTargetWaypoint != null)
                {
                    GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(newTargetWaypoint);
                    SetState(State.Undefined);
                    Update();
                }
                else
                {
                    GetRoleAnimation().GetWaypointPath().SetTargetWaypoint(null);
                    SetIdleState(false);
                }
            }
            else
            {
                // Do nothing
            }
        }

        private void JumpUpCompleteCB()
        {
            GetRoleAnimation().GetWaypointPath().CutOppisiteDirectionWaypointsOfSearchingResult(GetForward(), GetPosition());
            SetState(State.Undefined);
            Update();
        }

        private void OpenDoorCompleteCB()
        {
            // Delete openDoor type waypoint at opposite side of door.
            Waypoint newTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult();
            if (newTargetWaypoint != null && newTargetWaypoint.IsDoor())
            {
                GetRoleAnimation().GetWaypointPath().CutFirstWaypointOfSearchingResult();
            }

            if (newTargetWaypoint != null && newTargetWaypoint.IsDoor() &&
                DataCenter.query.ActorHasKeyItem(GetRoleAnimation().actor.o.pd, newTargetWaypoint.door.keyToDoorPairs) == false)
            {
                GetRoleAnimation().GetWaypointPath().ClearSearchingResult();
                GetRoleAnimation().GetOperation().SetIsLocked(false); 
            }

            SetState(State.Undefined);
            Update();
        }

        private void TurningCompleteCB()
        {
            if (IsInLadderUpStart() || IsInLadderDownStart())
            {
                // switch back to ladder state
                SetState(State.Ladder);
            }
            else
            {
                // normal turning
                SetState(State.Undefined);
                Update();
            }
        }

        private void ClimbingCompleteCB()
        {
            // Lock interaction operation until arrived at a moving type waypoint
            GetRoleAnimation().GetOperation().SetIsLocked(true);
            GetRoleAnimation().GetWaypointPath().CutOppisiteDirectionWaypointsOfSearchingResult(GetForward(), GetPosition());

            // Unlock interaction operation while path is empty after cut oppisite direction waypoint
            if (GetRoleAnimation().GetWaypointPath().NumberWaypointsOfSearchingResult() == 0)
            {
                GetRoleAnimation().GetOperation().SetIsLocked(false);
            }

            SetState(State.Undefined);
            Update();
        }

        private void JumpDownCompleteCB()
        {
            if (GetRoleAnimation().GetWaypointPath().CheckWhetherToCutOppisiteDirectionWaypointsOfSeatchingResult(GetForward(), GetPosition()))
            {
                GetRoleAnimation().GetWaypointPath().CutOppisiteDirectionWaypointsOfSearchingResult(GetForward(), GetPosition());
            }

            SetState(State.Undefined);
            Update();
        }

        private void IdleCompleteCB()
        {
            if (ContainsState(State.NoOperation))
            {
                SetIdleState(false);
            }
        }

        private void SkillCompleteCB()
        {
            SetState(State.Undefined);
            Update();
        }

        private void SoloCompleteCB()
        {
            if (recentSolo != SoloSM.Transition.StandingToCrouched)
            {
                SetState(State.Undefined);
                Update();
            }
        }

        private void DummyCompleteCB()
        {
            // Do nothing
            // it's just a placeholder
        }
    }
}
