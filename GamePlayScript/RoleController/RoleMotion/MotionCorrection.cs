using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    public class MotionCorrection : MonoBehaviour
    {
        public enum MovingFoot
        {
            Undefined,
            Left,
            Right
        }

        private RoleAnimation _roleAnimation = null;

        private Animator _animator = null;

        private MovingFoot _movingFoot = MovingFoot.Undefined;

        private float handIK_weight = 0;
        private Vector3 handIK_targetPosition = Vector3.zero;
        private bool handIK_isLeftHand = false;

        public void SetOpenDoorHandIKParams(bool handIK_isLeftHand)
        {
            this.handIK_isLeftHand = handIK_isLeftHand;
        }

        public void SetAnimator(Animator animator)
        {
            _animator = animator;
        }

        public Animator GetAnimator()
        {
            return _animator;
        }

        public void SetRoleAnimation(RoleAnimation roleAnimation)
        {
            _roleAnimation = roleAnimation;
        }

        public RoleAnimation GetRoleAnimation()
        {
            return _roleAnimation;
        }

        public bool AtRightHandSide(Vector3 referencePosition)
        {
            var pos = transform.position;
            referencePosition.y = pos.y;
            var refDir = referencePosition - pos;
            return IsRightwardDirection(refDir);
        }

        public bool AtLeftHandSide(Vector3 referencePosition)
        {
            var pos = transform.position;
            referencePosition.y = pos.y;
            var refDir = referencePosition - pos;
            return IsLeftwardDirection(refDir);
        }

        public bool IsLeftwardDirection(Vector3 referenceDirection)
        {
            referenceDirection.Normalize();
            return Vector3.Dot(transform.right, referenceDirection) < -0.8f;
        }

        public bool IsRightwardDirection(Vector3 referenceDirection)
        {
            referenceDirection.Normalize();
            return Vector3.Dot(transform.right, referenceDirection) > 0.8f;
        }

        public bool IsBackwardDirection(Vector3 referenceDirection)
        {
            referenceDirection.Normalize();
            return Vector3.Dot(transform.forward, referenceDirection) < -0.45f;
        }

        // animation event
        public void MovingFootEvent(int foot)
        {
            SetMovingFoot((MovingFoot)foot);
        }

        public MovingFoot GetMovingFoot()
        {
            return _movingFoot;
        }

        private void SetMovingFoot(MovingFoot foot)
        {
            _movingFoot = foot;
        }

        private void OnAnimatorMove()
        {
            if (GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.OpenDoor) == false)
            {
                HandIKAttenuate();
            }

            if (GetRoleAnimation().GetWaypointPath().GetTargetWaypoint() != null &&
                GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.Moving))
            {
                var targetWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();

                // IK for up and down stairs
                bool isStair = GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.IsStair);
                GetRoleAnimation().GetMotionAnimator().SetFootIK(isStair);

                float tweenSpeed = 5f;
                tweenSpeed *= Time.deltaTime;
                Vector3 rolePosition = GetRoleAnimation().GetMotionAnimator().GetPosition();
                Vector3 targetPosition = targetWaypoint.GetPosition();
                Vector3 targetDirection = targetPosition - rolePosition;
                Vector3 movingVelocity = GetAnimator().velocity;
                Vector3 faceDirection = GetRoleAnimation().GetMotionAnimator().GetForward();
                Vector3 tweenDirection = Vector3.Lerp(faceDirection, targetDirection, tweenSpeed);
                Vector3 moveToPosition = MoveTowards(rolePosition, targetPosition, movingVelocity.magnitude * Time.deltaTime, out bool isArrived);
                tweenDirection.y = 0;

                // Set as arrived when role is closed to target waypoint for a door type waypoint.
                // It can avoid role moving cross target waypoint.
                if (targetWaypoint.IsDoor())
                {
                    var hang = targetWaypoint.GetAttachedHangPoint();
                    if (hang != null)
                    {
                        Vector3 v1 = targetPosition - moveToPosition;
                        Vector3 v2 = hang.GetPosition() - moveToPosition;
                        if (Vector3.Dot(v1.normalized, v2.normalized) < 0)
                        {
                            isArrived = true;
                        }
                    }
                }

                GetRoleAnimation().GetMotionAnimator().SetForward(tweenDirection);
                GetRoleAnimation().GetMotionAnimator().SetPosition(moveToPosition);
                GetRoleAnimation().GetWaypointPath().SetIsArrivedTargetWaypoint(isArrived);

                // Unlock interactive operation while arrived at a moving type waypoint
                if (isArrived &&
                    GetRoleAnimation().GetWaypointPath().GetTargetWaypoint().type == Waypoint.Type.Moving)
                {
                    GetRoleAnimation().GetOperation().SetIsLocked(false);
                }
            }
            else
            {
                if (!GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.Idle) &&
                    !GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.Skill))
                {
                    // Control rotation and position in script when turn-left-90 or turn-right-90
                    if (GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.Turning) &&
                        (
                            GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.DirectionLeft) ||
                            GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.DirectionRight)
                        )
                       )
                    {
                        Quaternion rotation = GetRoleAnimation().GetMotionAnimator().GetRotation();
                        rotation *= GetAnimator().deltaRotation;
                        Vector3 position = GetRoleAnimation().GetMotionAnimator().GetPosition();
                        position += GetAnimator().deltaPosition * 0.25f; // weaken movement
                        GetRoleAnimation().GetMotionAnimator().SetRotation(rotation);
                        GetRoleAnimation().GetMotionAnimator().SetPosition(position);
                    }
                    else if (GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.Ladder))
                    {
                        Waypoint targetWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();
                        Waypoint previousTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetThePreviousWaypointInOriginalSearchingResult(targetWaypoint);
                        if (GetRoleAnimation().GetMotionAnimator().IsInLadderUpStart() ||
                            GetRoleAnimation().GetMotionAnimator().IsInLadderDownStart() ||
                            GetRoleAnimation().GetMotionAnimator().IsInLadderUpEnd() ||
                            GetRoleAnimation().GetMotionAnimator().IsInLadderDownEnd())
                        {
                            GetAnimator().ApplyBuiltinRootMotion();
                        }
                        else if (GetRoleAnimation().GetMotionAnimator().IsInLadderUpClimbing() ||
                                GetRoleAnimation().GetMotionAnimator().IsInLadderDownClimbing())
                        {
                            Vector3 velocity = GetAnimator().velocity;
                            Vector3 rolePosition = GetRoleAnimation().GetMotionAnimator().GetPosition();
                            Vector3 moveToPosition = MoveTowards(rolePosition, targetWaypoint.GetPosition(), velocity.magnitude * Time.deltaTime, out bool isArrived);
                            GetRoleAnimation().GetMotionAnimator().SetPosition(moveToPosition);
                            GetRoleAnimation().GetWaypointPath().SetIsArrivedTargetWaypoint(isArrived);
                            if (isArrived)
                            {
                                GetRoleAnimation().GetMotionAnimator().ExecuteCompleteCBManually(MotionAnimator.State.Ladder);
                            }
                        }
                        else
                        {
                            // Do nothing
                        }

                        // Keep facing to ladder side
                        Waypoint forwardReferenceWaypoint =
                            targetWaypoint != null && targetWaypoint.type == Waypoint.Type.Ladder ? targetWaypoint :
                            previousTargetWaypoint != null && previousTargetWaypoint.type == Waypoint.Type.Ladder ? previousTargetWaypoint : null;
                        if (forwardReferenceWaypoint != null)
                        {
                            GetRoleAnimation().GetMotionAnimator().SetForward(forwardReferenceWaypoint.GetForward());
                        }
                    }
                    else if (GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.OpenDoor))
                    {
                        var newTargetWaypoint = GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult();
                        if (newTargetWaypoint != null && newTargetWaypoint.IsDoor() &&
                            DataCenter.query.ActorHasKeyItem(GetRoleAnimation().actor.o.pd, newTargetWaypoint.door.keyToDoorPairs))
                        {
                            handIK_weight = newTargetWaypoint.door.HandIKWeight();
                            handIK_targetPosition = newTargetWaypoint.door.GetKnobPosition();

                            GetRoleAnimation().GetMotionAnimator().SetHandIK(
                                true, handIK_targetPosition, handIK_weight, handIK_isLeftHand
                            );

                            GetAnimator().ApplyBuiltinRootMotion();
                        }
                        else
                        {
                            GetRoleAnimation().GetMotionAnimator().SetHandIK(
                                true, newTargetWaypoint.door.GetKnobPosition(), GetAnimator().GetFloat("LockedDoorHandIKWeight"), false
                            );
                        }
                    }
                    else
                    {
                        GetAnimator().ApplyBuiltinRootMotion();
                    }
                }
            }

            GetRoleAnimation().GetMotionAnimator().MatchTargetUpdate();
            GetRoleAnimation().GetMotionAnimator().SyncAnimators();
        }

        private Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta, out bool isArrived)
        {
            isArrived = false;
            float num = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = target.z - current.z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            if (num4 == 0f || (maxDistanceDelta >= 0f && num4 <= maxDistanceDelta * maxDistanceDelta))
            {
                isArrived = true;
                return target;
            }
            float num5 = (float)System.Math.Sqrt(num4);
            return new Vector3(current.x + num / num5 * maxDistanceDelta, current.y + num2 / num5 * maxDistanceDelta, current.z + num3 / num5 * maxDistanceDelta);
        }

        private void HandIKAttenuate()
        {
            handIK_weight -= 5f * Time.deltaTime;
            if (handIK_weight <= 0)
            {
                GetRoleAnimation().GetMotionAnimator().SetHandIK(false, Vector3.zero, 0, false);
            }
            else
            {
                GetRoleAnimation().GetMotionAnimator().SetHandIK(true, handIK_targetPosition, handIK_weight, handIK_isLeftHand);
            }
        }
    }
}
