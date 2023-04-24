using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    // Up body animation(Two Arms)
    public class UpBodySM : UpBodySMBase
    {
        public enum Transition
        {
            None = 0,
            StickInHands = 1,
            Dynamic = 2
        }

        protected override int GetLayerIndex()
        {
            return 1;
        }

        protected override int GetAction(string clipName)
        {
            return Utils.EnumToValue(Utils.StringToEnum<Transition>(clipName));
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
