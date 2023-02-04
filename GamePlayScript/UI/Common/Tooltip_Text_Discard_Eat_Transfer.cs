using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace GameScript.UI.Common
{
    public class Tooltip_Text_Discard_Eat_Transfer : MonoBehaviour
    {
        public void Set(string txt, Action eatHandler, Action discardHandler, Action transferHandler)
        {
            gameObject.SetActive(true);
            text.text = txt == null ? string.Empty : txt;
            this.eatHandler = eatHandler;
            this.discardHandler = discardHandler;
            this.transferHandler = transferHandler;
            eatButton.SetVisible(eatHandler != null);
            discardButton.SetVisible(discardHandler != null);
            transferButton.SetVisible(transferHandler != null);
        }

        private void Start()
        {
            transferButton.SetClickedCB(TransferButtonClickedHandler);
            discardButton.SetClickedCB(DiscardButtonClickedHandler);
            eatButton.SetClickedCB(EatButtonClickedHandler);
        }

        #region Text

        [SerializeField]
        private TMP_Text _text = null;
        private TMP_Text text
        {
            get
            {
                return _text;
            }
        }

        #endregion

        #region Eat

        private Action eatHandler = null;

        [SerializeField]
        private GButton eatButton = null;

        private void EatButtonClickedHandler()
        {
            eatHandler?.Invoke();
        }

        #endregion

        #region Discard

        private Action discardHandler = null;

        [SerializeField]
        private GButton discardButton = null;

        private void DiscardButtonClickedHandler()
        {
            discardHandler?.Invoke();
        }

        #endregion

        #region Transfer

        private Action transferHandler = null;

        [SerializeField]
        private GButton transferButton = null;

        private void TransferButtonClickedHandler()
        {
            transferHandler?.Invoke();
        }

        #endregion

    }
}
