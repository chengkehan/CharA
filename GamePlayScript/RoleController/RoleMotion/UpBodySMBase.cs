using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public abstract class UpBodySMBase : ActionStateMachine
    {
        abstract protected string GetActionName();

        abstract protected int GetLayerIndex();

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
            notificationData.Reset();
            notificationData.roleId = GetRoleAnimation().actor.o.GetId();
            FillUpBodyAnimationCompleteND(notificationData);
            EventSystem.GetInstance().Notify(EventID.UpBodyAnimationComplete, notificationData);
        }

        abstract protected void FillUpBodyAnimationCompleteND(UpBodyAnimationCompleteND notificationData);

        public void BlendWeight(Animator animator, bool isBlendIn, bool isSmooth)
        {
            if (isSmooth)
            {
                var updater = new UpBodyBlendWeightUpdater(animator, isBlendIn, GetLayerIndex());
                Updaters.GetInstance().Add(updater);
            }
            else
            {
                animator.SetLayerWeight(GetLayerIndex(), isBlendIn ? 1 : 0);
            }
        }
    }
}
