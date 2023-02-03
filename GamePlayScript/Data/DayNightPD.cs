using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class DayNightPD : SerializableMonoBehaviourPD
    {
        [SerializeField]
        private float _dayNightProgress = 1;
        public float dayNightProgress
        {
            set
            {
                _dayNightProgress = Mathf.Clamp01(value);
            }
            get
            {
                return _dayNightProgress;
            }
        }
    }
}
