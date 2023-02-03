using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class BreakWallPD : SerializableMonoBehaviourIPD
    {
        [SerializeField]
        private float _health = 0;
        public float health
        {
            set
            {
                _health = value;
            }
            get
            {
                return _health;
            }
        }
    }
}
