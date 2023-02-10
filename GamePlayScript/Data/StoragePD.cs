using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameScript.Cutscene;

namespace GameScript
{
    [Serializable]
    public class StoragePD : SerializableMonoBehaviourPD
    {
        // All items in this storage
        [SerializeField]
        private List<ItemPD> allItems = new List<ItemPD>();

        public bool RemoveItem(string itemGUID)
        {
            for (int itemI = 0; itemI < allItems.Count; itemI++)
            {
                if (allItems[itemI].guid == itemGUID)
                {
                    allItems.RemoveAt(itemI);
                    return true;
                }
            }
            return false;
        }

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

        public bool AddItem(ItemPD itemPD)
        {
            if (ContainsItem(itemPD.guid))
            {
                return false;
            }
            else
            {
                allItems.Add(itemPD.Clone());
                return true;
            }
        }

        public bool AddItem(StorageConfig.ItemConfig item)
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
