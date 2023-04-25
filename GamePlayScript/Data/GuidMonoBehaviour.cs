using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    public class GuidMonoBehaviour : MonoBehaviour
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
    }
}
