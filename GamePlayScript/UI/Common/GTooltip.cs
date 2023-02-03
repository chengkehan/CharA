using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameScript.UI.Common
{
    public class GTooltip : ComponentBase, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private string tooltipLanguageKey = null;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (string.IsNullOrWhiteSpace(tooltipLanguageKey) == false)
            {
                ShowTooltip(GetLanguage(tooltipLanguageKey), TooltipType.Text);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (string.IsNullOrWhiteSpace(tooltipLanguageKey) == false)
            {
                HideTooltip();
            }
        }
    }
}
