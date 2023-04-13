using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class SoloCompleteND : NotificationData
    {
        public string roleId = null;

        public SoloSM.Transition transition = SoloSM.Transition.Undefined;
    }
}
