using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NaughtyAttributes;

namespace GameScript.Cutscene
{
    // Config in editor what items stored at here.
    [Serializable]
    public class StorageConfig
    {
        [Tooltip("How many items can be put into this storage.")]
        [SerializeField]
        [Min(0)]
        private int capacity = 3;

        [Tooltip("Items stored at here in config")]
        [SerializeField]
        private ItemConfig[] allItems = null;

        [Tooltip("What type of item can be stored at here.")]
        [SerializeField]
        [EnumFlags]
        private Define.ItemSpace legalItemSpace = Define.ItemSpace.One;

        private int NumberItems()
        {
            return allItems == null ? 0 : allItems.Length;
        }

        private ItemConfig GetItem(int index)
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

        // Add configured items to storage
        public void InitializeStorageItems(StoragePD storagePD)
        {
            if (storagePD != null)
            {
                for (int i = 0; i < NumberItems(); i++)
                {
                    var item = GetItem(i);
                    if (DataCenter.GetInstance().ContainsItemConfig(item.itemID))
                    {
                        if (string.IsNullOrWhiteSpace(item.itemGUID) == false)
                        {
                            // This item should not be taken out by roles
                            if (DataCenter.query.ItemAlreadyExistedInWorld(item.itemGUID) == false)
                            {
                                if (storagePD.ContainsItem(item.itemGUID) == false)
                                {
                                    var itemConfig = DataCenter.GetInstance().GetItemConfig(item.itemID);
                                    if (DataCenter.query.EnumMaskMatching(itemConfig.space, (int)legalItemSpace))
                                    {
                                        if (storagePD.NumberItems() < capacity)
                                        {
                                            storagePD.AddItem(item);
                                        }
                                        else
                                        {
                                            Utils.LogObservably("Storage initialization: capacity is full.");
                                        }
                                    }
                                    else
                                    {
                                        Utils.LogObservably("Storage initialization: ItemSpace mismatching, " + itemConfig.space + ", need " + legalItemSpace);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Utils.LogObservably("Storage initialization: Illegal itemGUID, " + item.itemGUID);
                        }
                    }
                    else
                    {
                        Utils.LogObservably("Storage initialization: Illegal itemID, " + item.itemID);
                    }
                }
            }
        }

        [Serializable]
        public class ItemConfig
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
