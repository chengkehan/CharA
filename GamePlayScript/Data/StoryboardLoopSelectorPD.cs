using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class StoryboardLoopSelectorPD : SerializableMonoBehaviourPD
    {
        [SerializeField]
        private int _currentIndex = 0;
        public int currentIndex
        {
            set
            {
                _currentIndex = value;
            }
            get
            {
                return _currentIndex;
            }
        }
    }
}
