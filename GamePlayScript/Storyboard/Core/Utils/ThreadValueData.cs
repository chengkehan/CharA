using System;
using UnityEngine;

namespace StoryboardCore
{
    [Serializable]
    public class ThreadValueData
    {
        [SerializeField]
        private string _valueName = string.Empty;
        public string valueName
        {
            private set
            {
                _valueName = value;
            }
            get
            {
                return _valueName;
            }
        }

        [SerializeField]
        private string _textValue = string.Empty;
        public string textValue
        {
            set
            {
                _textValue = value;
            }
            get
            {
                return _textValue;
            }

        }

        [SerializeField]
        private float _numberValue = 0;
        public float numberValue
        {
            set
            {
                _numberValue = value;
            }
            get
            {
                return _numberValue;
            }
        }

        [SerializeField]
        private bool _booleanValue = false;
        public bool booleanValue
        {
            set
            {
                _booleanValue = value;
            }
            get
            {
                return _booleanValue;
            }
        }

        public ThreadValueData(string name)
        {
            this.valueName = name;
        }
    }
}
