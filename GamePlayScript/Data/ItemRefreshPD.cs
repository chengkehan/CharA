using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class ItemRefreshPD : SerializableMonoBehaviourPD
    {
        [SerializeField]
        private int _refreshTimes = 0;
        public int refreshTimes
        {
            set
            {
                _refreshTimes = value;
            }
            get
            {
                return _refreshTimes;
            }
        }
    }
}
