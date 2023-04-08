using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameScript.UI.Common;
using UnityEngine.EventSystems;
using System;
using Febucci.UI;

namespace GameScript.UI.Talking
{
    public class WordsItem : ComponentBase, IPointerEnterHandler, IPointerExitHandler
    {
        public TMP_Text wordsText = null;

        public TMP_Text nameText = null;

        private Color textColor;

        private Color nameColor;

        private bool isSetAsGray = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isSetAsGray)
            {
                SetAsNormalInternal();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isSetAsGray)
            {
                SetAsGrayInternal();
            }
        }

        public void SetAsGray()
        {
            isSetAsGray = true;
            SetAsGrayInternal();
        }

        private void SetAsNormalInternal()
        {
            wordsText.color = textColor;
            nameText.color = nameColor;
            SetHeadIconVisible(true);
        }

        private void SetAsGrayInternal()
        {
            Color c = textColor;
            c.r *= 0.8f;
            c.g *= 0.8f;
            c.b *= 0.8f;
            wordsText.color = c;

            c = nameColor;
            c.r *= 0.8f;
            c.g *= 0.8f;
            c.b *= 0.8f;
            nameText.color = c;

            SetHeadIconVisible(false);
        }

        public void SetText(string name, string words, bool isFromChoice)
        {
            if (words == null)
            {
                words = string.Empty;
            }

            bool isOverlapingSound = string.IsNullOrWhiteSpace(name);
            bool isMe = name == GetLanguage("me");

            textColor = Color.white;
            if (isOverlapingSound)
            {
                textColor = new Color(0.8113208f, 0.3288811f, 0f, 1);
            }
            else
            {
                if (isMe == false)
                {
                    textColor = new Color(0.9863526f, 1f, 0.8160377f, 1);
                }
            }
            nameColor = Color.white;

            nameText.text = name;
            nameText.color = nameColor;

            if (isOverlapingSound)
            {
                wordsText.text = "<margin left=4%>      " + words;
            }
            else
            {
                wordsText.text = "<margin left=4%>            " + words;
            }
            wordsText.color = textColor;

            if (isOverlapingSound)
            {
                StartCoroutine(OverlapingSoundCoroutine());
            }
            else
            {
                if (isFromChoice == false)
                {
                    StartCoroutine(TypeCharsCoroutine(words));
                }
            }
        }

        #region Head Icon

        [SerializeField]
        private UnityEngine.Object _headIconPrefab = null;
        private UnityEngine.Object headIconPrefab
        {
            get
            {
                return _headIconPrefab;
            }
        }

        [SerializeField]
        private Transform _headIconBindingPoint = null;
        private Transform headIconBindingPoint
        {
            get
            {
                return _headIconBindingPoint;
            }
        }

        private GameObject _headIconGo = null;

        public void SetHeadIcon(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId) == false)
            {
                _headIconGo = Utils.InstantiateUIPrefab(headIconPrefab, headIconBindingPoint);
                var headIcon = _headIconGo.GetComponent<HeadIcon>();
                headIcon.LoadHeadIcon(roleId);
            }
        }

        private void SetHeadIconVisible(bool visible)
        {
            if (_headIconGo != null)
            {
                _headIconGo.SetActive(visible);
                var headIcon = _headIconGo.GetComponent<HeadIcon>();
                if (headIcon != null)
                {
                    headIcon.OverlayOthers(visible);
                }
            }
        }

        #endregion

        private IEnumerator TypeCharsCoroutine(string words)
        {
            wordsText.maxVisibleCharacters = 0;
            yield return null;
            wordsText.maxVisibleCharacters = 99999;

            TextAnimatorPlayer textAnimatorPlayer = GetComponent<TextAnimatorPlayer>();
            string wordsTextStr = "<margin left=4%>            " + words;
            textAnimatorPlayer.SetTypewriterSpeed(10);
            textAnimatorPlayer.ShowText(wordsTextStr);
        }

        private IEnumerator OverlapingSoundCoroutine()
        {
            wordsText.alpha = 0;
            yield return null;
            while(wordsText.alpha < 1)
            {
                wordsText.alpha += 1.5f * Time.deltaTime;
                yield return null;
            }
            wordsText.alpha = 1;
        }
    }
}