using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UEUI = UnityEngine.UI;
using TMPro;

namespace GameScript.UI.Common
{
    public class GButton : ComponentBase
    {
        public UEUI.Button button = null;

        public TMP_Text label = null;

        [SerializeField]
        private string languageKey = null;

        private Action _clickedCB = null;

        public void SetClickedCB(Action action)
        {
            _clickedCB = action;
        }

        public Action GetClickedCB()
        {
            return _clickedCB;
        }

        public void SetLabel(string label)
        {
            if (this.label != null)
            {
                this.label.text = label;
            }
        }

        public void SetColor(Color buttonColor, Color labelColor)
        {
            button.transition = UEUI.Selectable.Transition.ColorTint;

            UEUI.ColorBlock cb = button.colors;
            cb.colorMultiplier = 1;
            cb.fadeDuration = 0;
            cb.disabledColor = buttonColor;
            cb.highlightedColor = buttonColor;
            cb.normalColor = buttonColor;
            cb.pressedColor = buttonColor;
            cb.selectedColor = buttonColor;
            button.colors = cb;

            if (label != null)
            {
                label.color = labelColor;
            }
        }

        protected override void Start()
        {
            base.Start();

            button.onClick.AddListener(ButtonOnClickHandler);

            if (string.IsNullOrWhiteSpace(languageKey) == false)
            {
                SetLabel(GetLanguage(languageKey));
            }
        }

        private void ButtonOnClickHandler()
        {
            GetClickedCB()?.Invoke();
        }
    }
}