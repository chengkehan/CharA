using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace GameScript.UI.Common
{
    public class Tooltip_Text_Discard : Tooltip_Text
    {
        public Action discardHandler = null;

        [SerializeField]
        private GButton discardButton = null;

        protected virtual void Start()
        {
            discardButton.SetClickedCB(DiscardButtonClickedHandler);
        }

        private void DiscardButtonClickedHandler()
        {
            discardHandler?.Invoke();
        }
    }
}
