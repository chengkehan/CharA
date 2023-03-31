using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
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
