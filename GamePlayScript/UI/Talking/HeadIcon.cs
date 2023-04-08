using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameScript.UI.Common;

namespace GameScript.UI.Talking
{
    public class HeadIcon : ComponentBase
    {
        [SerializeField]
        private Image _iconImage = null;
        private Image iconImage
        {
            get
            {
                return _iconImage;
            }
        }

        public void LoadHeadIcon(string roleId)
        {
            iconImage.sprite = AssetsManager.GetInstance().LoadHeadIcon(roleId);
        }

        public void OverlayOthers(bool isOn)
        {
            if (gameObject != null)
            {
                Canvas panelCanvas = gameObject.GetComponentInParent<Canvas>();
                if (panelCanvas != null)
                {
                    Canvas overrideCanvas = gameObject.GetComponent<Canvas>();
                    if (overrideCanvas == null)
                    {
                        overrideCanvas = gameObject.AddComponent<Canvas>();
                    }
                    if (overrideCanvas != null)
                    {
                        if (isOn)
                        {
                            overrideCanvas.overrideSorting = true;
                            overrideCanvas.sortingOrder = panelCanvas.sortingOrder + 1;
                        }
                        else
                        {
                            overrideCanvas.overrideSorting = false;
                        }
                    }
                }
            }
        }
    }
}
