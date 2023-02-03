using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    public class SerializableMonoBehaviour<T> : MonoBehaviour
        where T : SerializableMonoBehaviourPD, new()
    {
        [Tooltip("GUID of Serializable MonoBehaviour")]
        [StoryboardCore.ReadOnly]
        [SerializeField]
        private string _guid = null;
        public string guid
        {
            private set
            {
                _guid = value;
            }
            get
            {
                return _guid;
            }
        }

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

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

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
