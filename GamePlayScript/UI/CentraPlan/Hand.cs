using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CUI = GameScript.UI.Common;

namespace GameScript.UI.CentraPlan.Hero
{
    public class Hand : CUI.ComponentBase, IPointerEnterHandler, IPointerExitHandler
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
        private Image _icon = null;
        private Image icon
        {
            get
            {
                return _icon;
            }
        }

        protected void ShowIcon(Sprite sprite)
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

            RefreshItem();

            EventSystem.GetInstance().AddListener(EventID.PickUpSceneItem, PickUpSceneItemHandler);
            EventSystem.GetInstance().AddListener(EventID.DropItemToScene, DropItemToSceneHandler);
            EventSystem.GetInstance().AddListener(EventID.TransferCardboardBoxItemToActor, TransferCardboardBoxItemToActorHandler);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            EventSystem.GetInstance().RemoveListener(EventID.PickUpSceneItem, PickUpSceneItemHandler);
            EventSystem.GetInstance().RemoveListener(EventID.DropItemToScene, DropItemToSceneHandler);
            EventSystem.GetInstance().RemoveListener(EventID.TransferCardboardBoxItemToActor, TransferCardboardBoxItemToActorHandler);
        }

        private void TransferCardboardBoxItemToActorHandler(NotificationData _data)
        {
            var data = _data as TransferCardboardBoxItemToActorND;
            if (data != null)
            {
                if (DataCenter.query.IsHeroActorGUID(data.actorGUID))
                {
                    RefreshItem();
                }
            }
        }

        private void DropItemToSceneHandler(NotificationData _data)
        {
            var data = _data as DropItemToSceneND;
            if (data != null)
            {
                if (DataCenter.query.IsHeroActorGUID(data.actorGUID))
                {
                    RefreshItem();
                }
            }
        }

        private void PickUpSceneItemHandler(NotificationData _data)
        {
            var data = _data as PickUpSceneItemND;
            if (data != null)
            {
                if (DataCenter.query.IsHeroActorGUID(data.actorGUID))
                {
                    RefreshItem();
                }
            }
        }

        protected void RefreshItem()
        {
            HideTooltip();
            HideIcon();
            HideHighlight();
            RefreshIcon();
        }

        protected virtual void RefreshIcon()
        {
            var heroActorPD = ActorsManager.GetInstance().GetHeroActor().pd;
            if (heroActorPD.inHandItem.IsEmpty() == false)
            {
                var icon = AssetsManager.GetInstance().LoadItemIcon(heroActorPD.inHandItem.itemID);
                ShowIcon(icon);
            }
        }

        protected virtual void DiscardItem()
        {
            var heroActorPD = ActorsManager.GetInstance().GetHeroActor().pd;
            if (heroActorPD.inHandItem.IsEmpty() == false)
            {
                ModeratorUtils.DropItemToScene(heroActorPD.guid.o, heroActorPD.inHandItem.guid);
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

        protected virtual void ShowTooltip()
        {
            string tooltip = GetLanguage("item_in_hand_slot");

            var heroActorPD = ActorsManager.GetInstance().GetHeroActor().pd;
            if (heroActorPD.inHandItem.IsEmpty())
            {
                tooltip += "\n" + GetLanguage("empty");
            }
            else
            {
                var itemConfig = DataCenter.GetInstance().GetItemConfig(heroActorPD.inHandItem.itemID);
                tooltip += "\n" + itemConfig.name;
                tooltip += "\n" + "<margin left=10%><size=95%>" + itemConfig.description + "</size></margin>";
                tooltip += "\n" + "<size=95%>" + GetLanguage("durability") + ": " + heroActorPD.inHandItem.durability.ToString("f1") + "</size>";
            }

            if (heroActorPD.inHandItem.IsEmpty())
            {
                ShowTooltip(tooltip);
            }
            else
            {
                ShowTooltip(tooltip, DiscardItem);
            }
        }

        private void ShowHighlight()
        {
            if (hightlightGo != null)
            {
                hightlightGo.SetActive(true);
            }

            ShowTooltip();
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
