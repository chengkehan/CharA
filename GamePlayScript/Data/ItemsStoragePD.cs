using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class ItemsStoragePD
    {
        [SerializeField]
        private OneItem[] allItems = null;

        public int NumberItems()
        {
            return allItems == null ? 0 : allItems.Length;
        }

        public OneItem GetItem(int index)
        {
            if (index < 0 || index >= allItems.Length)
            {
                return null;
            }
            else
            {
                return allItems[index];
            }
        }

        [Serializable]
        public class OneItem
        {
            [SerializeField]
            private string _itemGUID = null;
            public string itemGUID
            {
                get
                {
                    return _itemGUID;
                }
            }

            [SerializeField]
            private string _itemID = null;
            public string itemID
            {
                get
                {
                    return _itemID;
                }
            }
        }
    }
}
