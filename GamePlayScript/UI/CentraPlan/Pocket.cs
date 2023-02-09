using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CUI = GameScript.UI.Common;

namespace GameScript.UI.CentraPlan.Hero
{
    public class Pocket : CUI.ComponentBase, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private GameObject _highlightGo = null;
        private GameObject hightlightGo
        {
            get
            {
                return _highlightGo;
            }
        }

        [SerializeField]
        private Define.PocketType _pocketType = Define.PocketType.Clothes_Left_Side;
        private Define.PocketType pocketType
        {
            get
            {
                return _pocketType;
            }
        }

        [SerializeField]
        private Image _icon = null;
        private Image icon
        {
            get
            {
                return _icon;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowHighlight();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideHighlight();
        }

        protected override void Awake()
        {
            base.Awake();

            HideIcon();
        }

        protected override void Start()
        {
            base.Start();

            RefreshItemIcon();

            EventSystem.GetInstance().AddListener(EventID.PickUpSceneItem, PickupSceneItemHandler);
            EventSystem.GetInstance().AddListener(EventID.DropItemToScene, DropItemToSceneHandler);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            EventSystem.GetInstance().RemoveListener(EventID.PickUpSceneItem, PickupSceneItemHandler);
            EventSystem.GetInstance().RemoveListener(EventID.DropItemToScene, DropItemToSceneHandler);
        }

        private void DropItemToSceneHandler(NotificationData _data)
        {
            var data = _data as DropItemToSceneND;
            if (data != null)
            {
                if (DataCenter.query.IsHeroActorGUID(data.actorGUID))
                {
                    RefreshItemIcon();
                }
            }
        }

        private void PickupSceneItemHandler(NotificationData _data)
        {
            var data = _data as PickUpSceneItemND;
            if (data != null)
            {
                if (DataCenter.query.IsHeroActorGUID(data.actorGUID))
                {
                    RefreshItemIcon();
                }
            }
        }

        private void RefreshItemIcon()
        {
            HideTooltip();
            HideIcon();
            HideHighlight();

            // Load pocket icon
            {
                var heroActorPD = ActorsManager.GetInstance().GetHeroActor().pd;
                var pocketItemPD = heroActorPD.GetPocketItem((int)pocketType);
                if (pocketItemPD.IsEmpty() == false)
                {
                    AssetsManager.GetInstance().LoadItemIcon(pocketItemPD.itemID, (obj) =>
                    {
                        ShowIcon(obj);
                    });
                }
            }
        }

        private void TransferItem()
        {

        }

        private void EatItem()
        {
            Utils.Log("eat");
        }

        private void DiscardItem()
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

        protected override void OnUGUIEventSystemChanged(bool enabled)
        {
            base.OnUGUIEventSystemChanged(enabled);

            if (enabled == false)
            {
                HideHighlight();
            }
        }

        private void ShowIcon(Sprite sprite)
        {
            icon.gameObject.SetActive(true);
            icon.enabled = true;
            icon.sprite = sprite;
        }

        private void HideIcon()
        {
            icon.gameObject.SetActive(false);
            icon.enabled = false;
            icon.sprite = null;
        }

        private void ShowHighlight()
        {
            if (hightlightGo != null)
            {
                hightlightGo.SetActive(true);
            }


            // Show Tooltip
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

        private void HideHighlight()
        {
            if (hightlightGo != null)
            {
                hightlightGo.SetActive(false);
            }

            HideTooltip();
        }
    }
}
