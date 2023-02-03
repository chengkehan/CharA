using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using GameScript.UI.Common;
using Febucci.UI;

namespace GameScript.UI.Talking
{
    public class ChoiceItem : ComponentBase, IPointerClickHandler
    {
        public delegate void ClickedCB(int index);

        public TMP_Text wordsText = null;

        public TMP_Text indexText = null;

        private ClickedCB _clickedCB = null;

        private int _index = 0;

        public void OnPointerClick(PointerEventData eventData)
        {
            GetClickedCB()?.Invoke(GetIndex());
        }

        public void SetText(int index, string words)
        {
            if (words == null)
            {
                words = string.Empty;
            }

            SetIndex(index);

            indexText.text = "<margin left=4%>" + index + ".";
            wordsText.text = "<margin left=8%>  " + words;
            indexText.maxVisibleCharacters = 0;
            wordsText.maxVisibleCharacters = 0;

            StartCoroutine(DelayShowTextCoroutine(words));
        }

        public void SetClickedCB(ClickedCB action)
        {
            _clickedCB = action;
        }

        public ClickedCB GetClickedCB()
        {
            return _clickedCB;
        }

        public int GetIndex()
        {
            return _index;
        }

        private void SetIndex(int index)
        {
            _index = index;
        }

        private IEnumerator DelayShowTextCoroutine(string words)
        {
            yield return new WaitForSeconds(GetIndex() * 0.2f);

            indexText.maxVisibleCharacters = 99999;
            wordsText.maxVisibleCharacters = 99999;

            TextAnimatorPlayer textAnimatorPlayer = GetComponent<TextAnimatorPlayer>();
            string tag = TAnimTags.bh_Fade;
            string wordsTextStr = "<margin left=8%>  " + "{" + tag + "}" + words + "{" + tag + "}";
            textAnimatorPlayer.SetTypewriterSpeed(10);
            textAnimatorPlayer.ShowText(wordsTextStr);
        }
    }
}