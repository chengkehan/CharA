using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class SerializableMonoBehaviourPD
    {
        [SerializeField]
        private SetOnceObject<string> _guid = new SetOnceObject<string>();
        public SetOnceObject<string> guid
        {
            get
            {
                return _guid;
            }
        }
    }
}
