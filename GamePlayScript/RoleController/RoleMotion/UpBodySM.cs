using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    // Up body animation(Two Arms)
    public class UpBodySM : UpBodySMBase<UpBodySM.Transition>
    {
        public enum Transition
        {
            None = 0,
            StickInHands = 1,
            Dynamic = 2
        }

        protected override string GetActionName()
        {
            return "UpBodyAnimation";
        }

        protected override void FillUpBodyAnimationCompleteND(UpBodyAnimationCompleteND notificationData)
        {
            notificationData.upBodyAnimation = (Transition)GetAction();
        }
    }
}
