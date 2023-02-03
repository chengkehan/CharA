using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class PairsData
    {
        [Serializable]
        public class Pair
        {
            [SerializeField]
            private string[] _itemIDs = null;

            public int NumberItemIDs()
            {
                return _itemIDs == null ? 0 : _itemIDs.Length;
            }

            public string GetItemID(int index)
            {
                if (_itemIDs == null || index < 0 || index >= _itemIDs.Length)
                {
                    return null;
                }
                else
                {
                    return _itemIDs[index];
                }
            }
        }

        [SerializeField]
        private Pair[] _pairs = null;

        public int NumberPairs()
        {
            return _pairs == null ? 0 : _pairs.Length;
        }

        public Pair GetPair(int index)
        {
            if (_pairs == null || index < 0 || index >= _pairs.Length)
            {
                return null;
            }
            else
            {
                return _pairs[index];
            }
        }
    }
}
