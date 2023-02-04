using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class CardboardBoxPD : SerializableMonoBehaviourPD
    {
        // All items in this cardboardBox
        [SerializeField]
        private List<ItemPD> allItems = new List<ItemPD>();

        public int NumberItems()
        {
            return allItems.Count;
        }

        public ItemPD GetItem(int index)
        {
            if (index < 0 || index >= allItems.Count)
            {
                return null;
            }
            else
            {
                return allItems[index];
            }
        }

        public ItemPD GetItemByGUID(string itemGUID)
        {
            foreach (var item in allItems)
            {
                if (item.guid == itemGUID)
                {
                    return item;
                }
            }
            return null;
        }

        public bool ContainsItem(string itemGUID)
        {
            return GetItemByGUID(itemGUID) != null;
        }

        public bool AddItem(ItemsStoragePD.OneItem item)
        {
            if (ContainsItem(item.itemGUID))
            {
                return false;
            }
            else
            {
                if (DataCenter.GetInstance().ContainsItemConfig(item.itemID))
                {
                    var itemConfig = DataCenter.GetInstance().GetItemConfig(item.itemID);
                    var itemPD = new ItemPD(item.itemGUID, itemConfig);
                    allItems.Add(itemPD);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
