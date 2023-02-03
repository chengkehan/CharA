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

            HideHighlight();

            // Load item icon in hand
            {
                var heroActorPD = ActorsManager.GetInstance().GetHeroActor().pd;
                if (heroActorPD.inHandItem.IsEmpty() == false)
                {
                    AssetsManager.GetInstance().LoadItemIcon(heroActorPD.inHandItem.itemID, (obj)=>
                    {
                        ShowIcon(obj);
                    });
                }
            }
        }

        private void DiscardItem()
        {
            var heroActorPD = ActorsManager.GetInstance().GetHeroActor().pd;
            if (heroActorPD.inHandItem.IsEmpty() == false)
            {
                var notification = new DropItemToSceneND();
                notification.actorGUID = heroActorPD.guid.o;
                notification.itemGUID = heroActorPD.inHandItem.guid;
                EventSystem.GetInstance().Notify(EventID.DropItemToScene, notification);

                HideTooltip();
                HideIcon();
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

        private void ShowHighlight()
        {
            hightlightGo.SetActive(true);

            // ShowTooltip
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
                    ShowTooltip(tooltip, TooltipType.Text);
                }
                else
                {
                    ShowTooltip(tooltip, TooltipType.Text_Discard, () => { DiscardItem(); });
                }
            }

        }

        private void HideHighlight()
        {
            hightlightGo.SetActive(false);
            HideTooltip();
        }
    }
}
