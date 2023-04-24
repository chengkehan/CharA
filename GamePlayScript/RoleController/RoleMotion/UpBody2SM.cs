using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    // Up body animation Layer2(Two Arms and Head)
    public class UpBody2SM : UpBodySMBase
    {
        public enum Transition
        {
            None = 0,
            Headache = 1,
            Dynamic = 2
        }

        protected override int GetLayerIndex()
        {
            return 2;
        }

        protected override int GetAction(string clipName)
        {
            return Utils.EnumToValue(Utils.StringToEnum<Transition>(clipName));
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
