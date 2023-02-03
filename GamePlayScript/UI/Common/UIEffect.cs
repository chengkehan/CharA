using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UEUI = UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace GameScript.UI.Common
{
    public class UIEffect : ComponentBase, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public enum Type
        {
            Undefined = 0,
            ButtonColor = 1,
            TextColor = 2
        }

        // Is initialized in awake, change it art runtime is not allowed.
        public Type type = Type.Undefined;

        public Color hoverColor = Color.white;
        public Color pressColor = Color.white;

        private GButton button = null;
        private TMP_Text text = null;

        private bool isPointerInArea = false;
        private bool isPointerPressed = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerInArea = true;
            SetButtonColor(hoverColor);
            SetTextColor(hoverColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerInArea = false;
            SetButtonColor(Color.white);
            SetTextColor(Color.white);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerPressed = true;
            SetButtonColor(pressColor);
            SetTextColor(pressColor);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerPressed = false;
            SetButtonColor(isPointerInArea ? hoverColor : Color.white);
            SetTextColor(isPointerInArea ? hoverColor : Color.white);
        }

        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }

        private void Initialize()
        {
            if (type == Type.ButtonColor)
            {
                button = GetComponent<GButton>();
            }
            else if (type == Type.TextColor)
            {
                text = GetComponent<TMP_Text>();
            }
            else
            {
                Utils.Assert(false);
            }
        }

        private void SetButtonColor(Color color)
        {
            if (type == Type.ButtonColor && button != null)
            {
                button.SetColor(color, color);
            }
        }

        private void SetTextColor(Color color)
        {
            if (type == Type.TextColor && text != null)
            {
                text.color = color;
            }
        }
    }
}
