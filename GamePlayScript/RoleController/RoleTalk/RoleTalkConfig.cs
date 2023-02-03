using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    public class RoleTalkConfig : MonoBehaviour
    {
        [SerializeField]
        private string _storyboardName = null;
        public string storyboardName
        {
            get
            {
                return _storyboardName;
            }
        }
    }
}