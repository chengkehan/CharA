using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    public class SerializableMonoBehaviour<T> : GuidMonoBehaviour
        where T : SerializableMonoBehaviourPD, new()
    {
        private T _pd = null;
        public T pd
        {
            get
            {
                if (_pd == null)
                {
                    _pd = DataCenter.GetInstance().playerData.GetSerializableMonoBehaviourPD<T>(guid);
                }
                return _pd;
            }
        }

        public virtual void Save()
        {
            // Do nothing
        }

        protected virtual void Awake()
        {
            InitializeOnAwake();
        }

        protected virtual void InitializeOnAwake()
        {
            // Do nothing
        }

        protected virtual void Start()
        {
            InitializeOnStart();
        }

        protected virtual void InitializeOnStart()
        {
            // Do nothing
        }

        protected virtual void OnDestroy()
        {
            // Do nothing
        }
    }
}
