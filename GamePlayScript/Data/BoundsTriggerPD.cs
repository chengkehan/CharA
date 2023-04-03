using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class BoundsTriggerPD : SerializableMonoBehaviourPD
    {
        private int _triggeredTimes = 0;
        public int triggeredTimes
        {
            set
            {
                _triggeredTimes = value;
            }
            get
            {
                return _triggeredTimes;
            }
        }
    }
}
