using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CUI = GameScript.UI.Common;
using CBUI = GameScript.UI.CardboardBoxUI;

namespace GameScript.UI.CentraPlan.Hero
{
    public class Pocket : Hand
    {
        [SerializeField]
        private Define.PocketType _pocketType = Define.PocketType.Clothes_Left_Side;
        private Define.PocketType pocketType
        {
            get
            {
                return _pocketType;
            }
        }

        protected override void RefreshIcon()
        {
            var heroActorPD = ActorsManager.GetInstance().GetHeroActor().pd;
            var pocketItemPD = heroActorPD.GetPocketItem((int)pocketType);
            if (pocketItemPD.IsEmpty() == false)
            {
                var icon = AssetsManager.GetInstance().LoadItemIcon(pocketItemPD.itemID);
                ShowIcon(icon);
            }
        }

        private void TransferItem()
        {
            var heroActorPD = ActorsManager.GetInstance().GetHeroActor().pd;
            var pocketItemPD = heroActorPD.GetPocketItem((int)pocketType);
            if (pocketItemPD.IsEmpty() == false)
            {
                if (UIManager.GetInstance().ContainsUI(UIManager.UIName.CardboardBox))
                {
                    var cardboardBoxUI = UIManager.GetInstance().GetUI<CBUI.CardboardBoxUI>(UIManager.UIName.CardboardBox);
                    var notificationData = new TransferPocketItemToCardboardBoxND();
                    notificationData.actorGUID = heroActorPD.guid.o;
                    notificationData.itemGUID = pocketItemPD.guid;
                    notificationData.cardboardBoxGUID = cardboardBoxUI.cardboardBox.guid;
                    EventSystem.GetInstance().Notify(EventID.TransferPocketItemToCardboardBox, notificationData);
                }
            }
        }

        private void EatItem()
        {
            Utils.Log("eat");
        }

        protected override void DiscardItem()
        {
            var heroActorPD = ActorsManager.GetInstance().GetHeroActor().pd;
            var pocketItemPD = heroActorPD.GetPocketItem((int)pocketType);
            if (pocketItemPD.IsEmpty() == false)
            {
                var notification = new DropItemToSceneND();
                notification.actorGUID = heroActorPD.guid.o;
                notification.itemGUID = pocketItemPD.guid;
                EventSystem.GetInstance().Notify(EventID.DropItemToScene, notification);
            }
        }

        protected override void Start()
        {
            base.Start();

            EventSystem.GetInstance().AddListener(EventID.TransferPocketItemToCardboardBox, TransferPocketItemToCardboardBoxHandler);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            EventSystem.GetInstance().RemoveListener(EventID.TransferPocketItemToCardboardBox, TransferPocketItemToCardboardBoxHandler);
        }

        private void TransferPocketItemToCardboardBoxHandler(NotificationData _data)
        {
            var data = _data as TransferPocketItemToCardboardBoxND;
            if (data != null)
            {
                if (DataCenter.query.IsHeroActorGUID(data.actorGUID))
                {
                    RefreshItem();
                }
            }
        }

        protected override void ShowTooltip()
        {
            var tipText = string.Empty;
            if (pocketType == Define.PocketType.Clothes_Left_Side)
            {
                tipText = GetLanguage("cloths_left_pocket");
            }
            if (pocketType == Define.PocketType.Clothes_Right_Side)
            {
                tipText = GetLanguage("cloths_right_pocket");
            }
            if (pocketType == Define.PocketType.Trousers_Left_Side)
            {
                tipText = GetLanguage("trousers_left_pocket");
            }
            if (pocketType == Define.PocketType.Trousers_Right_Side)
            {
                tipText = GetLanguage("trousers_right_pocket");
            }

            var pocketItemPD = ActorsManager.GetInstance().GetHeroActor().pd.GetPocketItem((int)pocketType);
            if (pocketItemPD.IsEmpty())
            {
                tipText += "\n" + GetLanguage("empty");
            }
            else
            {
                var itemConfig = DataCenter.GetInstance().GetItemConfig(pocketItemPD.itemID);
                tipText += "\n" + itemConfig.name;
                tipText += "\n" + "<margin left=10%><size=95%>" + itemConfig.description + "</size></margin>";
            }

            if (pocketItemPD.IsEmpty())
            {
                ShowTooltip(tipText);
            }
            else
            {
                var itemConfig = DataCenter.GetInstance().GetItemConfig(pocketItemPD.itemID);
                if (itemConfig.eatable)
                {
                    if (UIManager.GetInstance().ContainsUI(UIManager.UIName.CardboardBox))
                    {
                        ShowTooltip(tipText, DiscardItem, EatItem, TransferItem);
                    }
                    else
                    {
                        ShowTooltip(tipText, DiscardItem, EatItem);
                    }

                }
                else
                {
                    if (UIManager.GetInstance().ContainsUI(UIManager.UIName.CardboardBox))
                    {
                        ShowTooltip(tipText, DiscardItem, null, TransferItem);
                    }
                    else
                    {
                        ShowTooltip(tipText, DiscardItem);
                    }
                }
            }
        }
    }
}
