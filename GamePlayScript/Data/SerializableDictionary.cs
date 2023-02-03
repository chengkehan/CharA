using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : SerializableDictionaryReadOnly<TKey, TValue>
    {
        public void Add(TKey key, TValue value)
        {
            data.Add(key, value);
        }

        protected override void SetValue(TKey key, TValue value)
        {
            data[key] = value;
        }
    }
}
