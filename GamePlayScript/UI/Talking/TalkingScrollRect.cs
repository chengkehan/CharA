using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UE = UnityEngine;
using UEUI = UnityEngine.UI;
using GameScript.UI.Common;

namespace GameScript.UI.Talking
{
    public class TalkingScrollRect : ComponentBase
    {
        public UEUI.ScrollRect scrollRect = null;

        public Transform container = null;

        public UE.Object spacerPrefab = null;

        public UE.Object wordsItemPrefab = null;

        public UE.Object buttonPrefab = null;

        public UE.Object choiceItemPrefab = null;

        public UE.Object nextStepPrefab = null;

        private PropertyInfo _vScrollingNeededProperty = null;

        private Action<float> _scrollValueChangedCB = null;

        private ComponentBase _topSpacer = null;

        private ComponentBase _bottomSpacer = null;

        private List<ComponentBase> allItems = new List<ComponentBase>();

        public void AddWordsItem(string name, string words, bool isFromChoice)
        {
            var wordsItem = Utils.InstantiateUIPrefab(wordsItemPrefab, container).GetComponent<WordsItem>();
            allItems.Add(wordsItem);
            wordsItem.SetText(name, words, isFromChoice);
            wordsItem.InsertBefore(GetBottomSpacer());
        }

        public void AddButton(string label, Action clickedCB)
        {
            var button = Utils.InstantiateUIPrefab(buttonPrefab, container).GetComponent<GButton>();
            allItems.Add(button);
            button.SetLabel(label);
            button.SetClickedCB(clickedCB);
            button.InsertBefore(GetBottomSpacer());
        }

        public void AddNextStep(Action clickedCB)
        {
            var button = Utils.InstantiateUIPrefab(nextStepPrefab, container).GetComponent<NextStep>();
            allItems.Add(button);
            button.SetClickedCB(clickedCB);
            button.InsertBefore(GetBottomSpacer());
        }

        public void AddChoiceItem(int index, string words, ChoiceItem.ClickedCB clickedCB)
        {
            var choiceItem = Utils.InstantiateUIPrefab(choiceItemPrefab, container).GetComponent<ChoiceItem>();
            allItems.Add(choiceItem);
            choiceItem.SetText(index, words);
            choiceItem.InsertBefore(GetBottomSpacer());
            choiceItem.SetClickedCB(clickedCB);
        }

        public void RemoveTheLastItem()
        {
            if (allItems.Count > 0)
            {
                var theLastItem = allItems[allItems.Count - 1];
                allItems.RemoveAt(allItems.Count - 1);
                Utils.Destroy(theLastItem.gameObject);
            }
        }

        public void SetAllItemsAsGray()
        {
            foreach (var item in allItems)
            {
                if (item is WordsItem)
                {
                    (item as WordsItem).SetAsGray();
                }
            }
        }

        public void SetScrollPosition(float v)
        {
            scrollRect.verticalNormalizedPosition = v;
        }

        public float GetScrollPosition()
        {
            return scrollRect.verticalNormalizedPosition;
        }

        public bool IsScrollingNeeded()
        {
            return (bool)_vScrollingNeededProperty.GetValue(scrollRect);
        }

        public void SetScrollValueChangedCB(Action<float> cb)
        {
            _scrollValueChangedCB = cb;
        }

        public Action<float> GetScrollValueChangedCB()
        {
            return _scrollValueChangedCB;
        }

        protected override void Awake()
        {
            base.Awake();

            _vScrollingNeededProperty = typeof(UEUI.ScrollRect).GetProperty("vScrollingNeeded", BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance);
            scrollRect.onValueChanged.AddListener(OnScrollValueChangedHandler);

            InitializeSpacers();
        }

        private void OnScrollValueChangedHandler(Vector2 scrollValue)
        {
            GetScrollValueChangedCB()?.Invoke(scrollValue.y);
        }

        private void InitializeSpacers()
        {
            if (_topSpacer == null)
            {
                _topSpacer = Utils.InstantiateUIPrefab(spacerPrefab, container).GetComponent<ComponentBase>();
            }

            if (_bottomSpacer == null)
            {
                _bottomSpacer = Utils.InstantiateUIPrefab(spacerPrefab, container).GetComponent<ComponentBase>();
            }
        }

        private ComponentBase GetTopSpacer()
        {
            return _topSpacer;
        }

        private ComponentBase GetBottomSpacer()
        {
            return _bottomSpacer;
        }
    }
}