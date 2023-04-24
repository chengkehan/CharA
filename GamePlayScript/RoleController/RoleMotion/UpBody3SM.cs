using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    // Up body animation Layer2(Two Arms, Head and Spine)
    public class UpBody3SM : UpBodySMBase
    {
        public enum Transition
        {
            None = 0,
            Dynamic = 1
        }

        protected override int GetLayerIndex()
        {
            return 3;
        }

        protected override int GetAction(string clipName)
        {
            return Utils.EnumToValue(Utils.StringToEnum<Transition>(clipName));
        }

        protected override string GetActionName()
        {
            return "UpBodyAnimation3";
        }

        protected override void FillUpBodyAnimationCompleteND(UpBodyAnimationCompleteND notificationData)
        {
            notificationData.upBodyAnimation3 = (Transition)GetAction();
        }
    }
}
