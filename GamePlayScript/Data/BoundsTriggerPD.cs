using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class BoundsTriggerPD : SerializableMonoBehaviourPD
    {
        [SerializeField]
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
