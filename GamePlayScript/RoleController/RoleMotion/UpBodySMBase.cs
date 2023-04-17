using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public abstract class UpBodySMBase<T> : ActionStateMachine
        where T : Enum
    {
        abstract protected string GetActionName(); 

        protected override int GetAction(string clipName)
        {
            return Utils.EnumToValue(Utils.StringToEnum<T>(clipName));
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash(GetActionName());
        }

        protected override void InitializeIgnoreStateNames()
        {
            base.InitializeIgnoreStateNames();
            AddIgnoreStateName("Dummy");
        }

        public override void Initialize()
        {
            base.Initialize();

            SetActionCompleteCB(AnimationCompleteCB);
        }

        private void AnimationCompleteCB()
        {
            var notificationData = DataCenter.GetInstance().cache.upBodyAnimationCompleteND;
            notificationData.roleId = GetRoleAnimation().actor.o.GetId();
            notificationData.upBodyAnimation = 0;
            notificationData.upBodyAnimation2 = 0;
            FillUpBodyAnimationCompleteND(notificationData);
            EventSystem.GetInstance().Notify(EventID.UpBodyAnimationComplete, notificationData);
        }

        abstract protected void FillUpBodyAnimationCompleteND(UpBodyAnimationCompleteND notificationData);
    }
}
