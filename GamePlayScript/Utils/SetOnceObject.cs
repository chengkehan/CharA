using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class SetOnceObject<T>
    {
        [SerializeField]
        private T obj = default(T);

        [SerializeField]
        private bool _initialized = false;
        public bool initialized
        {
            private set
            {
                _initialized = value;
            }
            get
            {
                return _initialized;
            }
        }

        public T o
        {
            get
            {
                return obj;
            }
            set
            {
                if (initialized)
                {
                    throw new InvalidOperationException("It can't be set twice.");
                }
                else
                {
                    initialized = true;
                    obj = value;
                }
            }
        }
    }
}
