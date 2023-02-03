using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace GameScript.UI.Common
{
    public class Tooltip_Text_Discard_Eat : Tooltip_Text_Discard
    {
        public Action eatHandler = null;

        [SerializeField]
        private GButton eatButton = null;

        protected override void Start()
        {
            base.Start();

            eatButton.SetClickedCB(EatButtonClickedHandler);
        }

        private void EatButtonClickedHandler()
        {
            eatHandler?.Invoke();
        }
    }
}
