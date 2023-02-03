using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class SequencePlayerPD : SerializableMonoBehaviourPD
    {
        [SerializeField]
        private bool _isPlayed = false;
        public bool isPlayed
        {
            set
            {
                _isPlayed = value;
            }
            get
            {
                return _isPlayed;
            }
        }
    }
}
