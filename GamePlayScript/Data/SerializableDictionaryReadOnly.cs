using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class SerializableDictionaryReadOnly<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = null;

        [SerializeField]
        private List<TValue> values = null;

        protected Dictionary<TKey, TValue> data = null;

        public SerializableDictionaryReadOnly()
        {
            data = new Dictionary<TKey, TValue>();
        }

        public bool ContainsKey(TKey key)
        {
            return data.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return data.TryGetValue(key, out value);
        }

        public int Count
        {
            get
            {
                return keys == null || values == null ? 0 : keys.Count;
            }
        }

        public TValue this[int index]
        {
            get
            {
                if (keys == null || values == null || keys.Count != values.Count || index < 0 || index >= values.Count)
                {
                    return default(TValue);
                }
                else
                {
                    return values[index];
                }
            }
        }

        public TValue this[TKey key]
        {
            set
            {
                SetValue(key, value);
            }
            get
            {
                return data[key];
            }
        }

        public void OnAfterDeserialize()
        {
            if (keys != null)
            {
                data.Clear();
                int length = keys.Count;
                for (int i = 0; i < length; i++)
                {
                    data.Add(keys[i], values[i]);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            if (data != null)
            {
                keys = new List<TKey>();
                values = new List<TValue>();
                keys.AddRange(data.Keys);
                values.AddRange(data.Values);
            }
        }

        protected virtual void SetValue(TKey key, TValue value)
        {
            throw new InvalidOperationException("Set value is not allowed. It's readonly.");
        }
    }
}
