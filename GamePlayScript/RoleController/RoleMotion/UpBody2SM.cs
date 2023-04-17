using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    // Up body animation Layer2(Two Arms and Head)
    public class UpBody2SM : UpBodySMBase<UpBody2SM.Transition>
    {
        public enum Transition
        {
            None = 0,
            Headache = 1,
            Dynamic = 2
        }

        protected override string GetActionName()
        {
            return "UpBodyAnimation2";
        }

        protected override void FillUpBodyAnimationCompleteND(UpBodyAnimationCompleteND notificationData)
        {
            notificationData.upBodyAnimation2 = (Transition)GetAction();
        }
    }
}
