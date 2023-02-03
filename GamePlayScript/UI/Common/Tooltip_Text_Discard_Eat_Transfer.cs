using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace GameScript.UI.Common
{
    public class Tooltip_Text_Discard_Eat_Transfer : Tooltip_Text_Discard_Eat
    {
        public Action transferHandler = null;

        [SerializeField]
        private GButton transferButton = null;

        protected override void Start()
        {
            base.Start();

            transferButton.SetClickedCB(TransferButtonClickedHandler);
        }

        private void TransferButtonClickedHandler()
        {
            transferHandler?.Invoke();
        }
    }
}
