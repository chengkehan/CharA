using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public abstract class ActionStateMachine : StateMachineBehaviour
    {
        public delegate void ActionCompleteCB();

        private ActionCompleteCB _actionCompleteCB = null;

        private RoleAnimation _roleAnimation = null;

        private int _action = 0;

        private Dictionary<int, float> _completeTimeOfActions = null;

        private Dictionary<int, int> _sequenceActions = null;

        private List<int[]> _equalActions = null;

        private List<AnimatorClipInfo> clipInfos = new List<AnimatorClipInfo>();

        private List<string> _ignoreStateNames = null;

        private int _actionNameId = 0;

        public virtual void Initialize()
        {
            // Do nothing
        }

        public void SetRoleAnimation(RoleAnimation roleAnimation)
        {
            _roleAnimation = roleAnimation;
        }

        public RoleAnimation GetRoleAnimation()
        {
            return _roleAnimation;
        }

        public bool IsInTransition(Animator animator)
        {
            if (animator == null)
            {
                return false;
            }
            else
            {
                return animator.IsInTransition(0);
            }
        }

        public void SetActionCompleteCB(ActionCompleteCB cb)
        {
            _actionCompleteCB = cb;
        }
        public ActionCompleteCB GetActionCompleteCB()
        {
            return _actionCompleteCB;
        }
        public void ExecuteActionCompleteCB()
        {
            GetActionCompleteCB()?.Invoke();
        }

        public void SetAction(Animator animator, int action)
        {
            if (animator != null)
            {
                if (AreEqualActions(action, _action) == false)
                {
                    _action = action;
                    animator.SetInteger(GetActionNameId(), _action);
                }
            }
        }

        public int GetAction()
        {
            return _action;
        }

        abstract protected int GetAction(string clipName);

        public int GetActionNameId()
        {
            if (_actionNameId == 0)
            {
                _actionNameId = InitializeActionNameId();
            }
            return _actionNameId;
        }

        public bool IsActionComplete(System.Enum action, Animator animator)
        {
            if (animator == null)
            {
                return true;
            }
            else
            {
                
                int actionVal = Utils.EnumToValue(action);
                while (GetSequenceAction(actionVal, out int sequenceAction))
                {
                    if (GetAction() == actionVal)
                    {
                        return false;
                    }

                    actionVal = sequenceAction;
                }
                if (GetAction() == actionVal)
                {
                    if (IsInTransition(animator))
                    {
                        return false;
                    }
                    else
                    {
                        var completeTime = GetCompleteTimeOfAction(actionVal);
                        var animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                        if (animStateInfo.normalizedTime >= completeTime)
                        {
                            animator.GetCurrentAnimatorClipInfo(0, clipInfos);
                            if (clipInfos.Count > 0 && clipInfos[0].clip != null)
                            {
                                var currentActionVal = GetAction(clipInfos[0].clip.name);
                                return currentActionVal == actionVal;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var animStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

            if (MatchingIgnoreStateNames(ref animStateInfo))
            {
                return;
            }

            var completeTime = GetCompleteTimeOfAction(GetAction());
            if (animStateInfo.normalizedTime >= completeTime && IsInTransition(animator) == false)
            {
                if (GetSequenceAction(GetAction(), out int sequenceAction))
                {
                    SetAction(animator, sequenceAction);
                }
                else
                {
                    if (animStateInfo.loop == false)
                    {
                        ExecuteActionCompleteCB();
                    }
                    else
                    {
                        LoopTypeActionCompleteFirstCircle(GetAction());
                    }
                }
            }
        }

        protected virtual void LoopTypeActionCompleteFirstCircle(int action)
        {
            // Do nothing
        }

        public virtual void MatchTargetUpdate(Animator animator)
        {
            // Do nothing
        }

        abstract protected int InitializeActionNameId();

        protected virtual void InitializeCompleteTimeOfActions()
        {
            // Do nothing
        }

        protected void AddCompleteTimeOfAction(System.Enum action, float time)
        {
            if (_completeTimeOfActions == null)
            {
                _completeTimeOfActions = new Dictionary<int, float>();
            }
            _completeTimeOfActions.Add(Utils.EnumToValue(action), time);
        }

        protected virtual void InitializeSequenceActions()
        {
            // Do nothing
        }

        protected void AddSequenceAction(System.Enum action1, System.Enum action2)
        {
            if (_sequenceActions == null)
            {
                _sequenceActions = new Dictionary<int, int>();
            }
            _sequenceActions.Add(Utils.EnumToValue(action1), Utils.EnumToValue(action2));
        }

        protected virtual void InitializedEqualActions()
        {
            // Do nothing
        }

        protected void AddEuqalActions(System.Enum action1, System.Enum action2)
        {
            if (_equalActions == null)
            {
                _equalActions = new List<int[]>();
            }

            _equalActions.Add(new int[] { Utils.EnumToValue(action1), Utils.EnumToValue(action2) });
        }

        protected virtual void InitializeIgnoreStateNames()
        {
            // Do nothing
        }

        protected void AddIgnoreStateName(string stateName)
        {
            if (_ignoreStateNames == null)
            {
                _ignoreStateNames = new List<string>();
            }
            _ignoreStateNames.Add(stateName);
        }

        protected bool MatchTarget(
            Animator animator, string stateName, AvatarTarget targetBodyPart,
            float matchPositionX, float matchPositionY, float matchPositionZ, Quaternion matchRotation,
            float matchPositionXWeight, float matchPositionYWeight, float matchPositionZWeight, float matchRotationWeight,
            float startNormalizedTime, float targetNormalizedTime)
        {
            if (animator != null && animator.isMatchingTarget == false && animator.IsInTransition(0) == false && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                animator.MatchTarget(
                    new Vector3(matchPositionX, matchPositionY, matchPositionZ), matchRotation, targetBodyPart,
                    new MatchTargetWeightMask(
                        new Vector3(matchPositionXWeight, matchPositionYWeight, matchPositionZWeight),
                        matchRotationWeight),
                    startNormalizedTime, targetNormalizedTime
                );
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AreEqualActions(int action1, int action2)
        {
            if (action1 == action2)
            {
                return true;
            }

            if (_equalActions == null)
            {
                InitializedEqualActions();
            }

            if (_equalActions == null)
            {
                return false;
            }
            else
            {
                foreach (var item in _equalActions)
                {
                    if (item != null && item[0] == action1 && item[1] == action2)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private float GetCompleteTimeOfAction(int action)
        {
            if (_completeTimeOfActions == null)
            {
                InitializeCompleteTimeOfActions();
            }
            if (_completeTimeOfActions == null || _completeTimeOfActions.ContainsKey(action) == false)
            {
                return 1.0f;
            }
            else
            {
                return _completeTimeOfActions[action];
            }
        }

        private bool GetSequenceAction(int action1, out int action2)
        {
            if (_sequenceActions == null)
            {
                InitializeSequenceActions();
            }
            if (_sequenceActions == null || _sequenceActions.ContainsKey(action1) == false)
            {
                action2 = 0;
                return false;
            }
            else
            {
                action2 = _sequenceActions[action1];
                return true;
            }
        }

        private bool MatchingIgnoreStateNames(ref AnimatorStateInfo animatorStateInfo)
        {
            InitializeIgnoreStateNames();

            if (_ignoreStateNames != null)
            {
                foreach (var ignoreStateName in _ignoreStateNames)
                {
                    if (animatorStateInfo.IsName(ignoreStateName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
