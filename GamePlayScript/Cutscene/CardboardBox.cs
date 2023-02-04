using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class CardboardBox : PickableObject<CardboardBoxPD>
    {
        [Tooltip("Config in editor what items stored at here.")]
        [SerializeField]
        private ItemsStoragePD itemsStorage = null;

        protected override void InitializeOnStart()
        {
            base.InitializeOnStart();

            if (itemsStorage != null)
            {
                for (int i = 0; i < itemsStorage.NumberItems(); i++)
                {
                    var item = itemsStorage.GetItem(i);
                    if (DataCenter.GetInstance().ContainsItemConfig(item.itemID))
                    {
                        if (string.IsNullOrWhiteSpace(item.itemGUID) == false)
                        {
                            if (DataCenter.query.ItemAlreadyExistedInWorld(item.itemGUID) == false)
                            {
                                if (pd.ContainsItem(item.itemGUID) == false)
                                {
                                    pd.AddItem(item);
                                }
                            }
                        }
                        else
                        {
                            Utils.Log("CardboardBox initialization: Illegal itemGUID, " + item.itemGUID);
                        }
                    }
                    else
                    {
                        Utils.LogObservably("CardboardBox initialization: Illegal itemID, " + item.itemID);
                    }
                }
            }
        }
    }
}
