using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class UpBodyAnimationCompleteND : NotificationData
    {
        public string roleId = null;

        public UpBodySM.Transition upBodyAnimation = UpBodySM.Transition.None;

        public UpBody2SM.Transition upBodyAnimation2 = UpBody2SM.Transition.None;

        public UpBody3SM.Transition upBodyAnimation3 = UpBody3SM.Transition.None;

        public void Reset()
        {
            upBodyAnimation = UpBodySM.Transition.None;
            upBodyAnimation2 = UpBody2SM.Transition.None;
            upBodyAnimation3 = UpBody3SM.Transition.None;
        }
    }
}
