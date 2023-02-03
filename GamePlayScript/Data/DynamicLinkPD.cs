using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class DynamicLinkPD : SerializableMonoBehaviourPD
    {
        [SerializeField]
        private bool _isLinked = false;
        public bool isLinked
        {
            set
            {
                _isLinked = value;
            }
            get
            {
                return _isLinked;
            }
        }
    }
}
