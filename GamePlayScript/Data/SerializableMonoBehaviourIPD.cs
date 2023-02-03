using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class SerializableMonoBehaviourIPD : SerializableMonoBehaviourPD
    {
        [SerializeField]
        private SetOnceObject<bool> _initialized = new SetOnceObject<bool>();
        public SetOnceObject<bool> initialized
        {
            set
            {
                _initialized = value;
            }
            get
            {
                return _initialized;
            }
        }
    }
}
