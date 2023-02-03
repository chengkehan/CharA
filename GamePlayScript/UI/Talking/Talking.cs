using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameScript.UI.Common;

namespace GameScript.UI.Talking
{
    public class Talking : UIBase
    {
        public Scroller scroller = null;

        public TalkingScrollRect scrollRect = null;

        public void AddWords(string name, string words, bool isFromChoice)
        {
            scrollRect.AddWordsItem(name, words, isFromChoice);
        }

        public void AddButton(string label, Action clickedCB)
        {
            scrollRect.AddButton(label, clickedCB);
        }

        public void AddNextStep(Action clickedCB)
        {
            scrollRect.AddNextStep(clickedCB);
        }

        public void AddChoice(int index, string words, ChoiceItem.ClickedCB clickedCB)
        {
            scrollRect.AddChoiceItem(index, words, clickedCB);
        }

        public void RemoveTheLastOne()
        {
            scrollRect.RemoveTheLastItem();
        }

        public void SetAllItemsAsGray()
        {
            scrollRect.SetAllItemsAsGray();
        }

        public void ScrollToBottom()
        {
            StartCoroutine(ScrollToBottomCoroutine());
        }

        private IEnumerator ScrollToBottomCoroutine()
        {
            yield return null;
            if (scrollRect != null && scrollRect.IsScrollingNeeded())
            {
                scrollRect.SetScrollPosition(0);
            }
            yield return null;
            if (scroller != null)
            {
                scroller.SetScrollPosition(1);
            }
        }

        // Test Only
        //private void Start()
        //{
        //    AddWords("雷探长", "“叽里呱啦叽里呱啦叽里呱啦叽里呱啦叽里呱啦叽里呱啦叽里呱啦叽里呱啦叽里呱啦叽里呱啦”");
        //    AddButton("打开看看", null);
        //    AddChoice(1, "走左边的那条路或许会更好一点。", null);
        //    AddChoice(2, "我不知道该怎么选择。", null);
        //}

        private void Awake()
        {
            scrollRect.SetScrollValueChangedCB(ScrollValueChangedCB);
            scroller.SetScrollValueChangedCB(ScrollValueChangedCB2);
        }

        private void ScrollValueChangedCB2(float scrollValue)
        {
            if (scrollRect.IsScrollingNeeded())
            {
                scrollRect.SetScrollPosition(1 - scrollValue);
            }
        }

        private void ScrollValueChangedCB(float scrollValue)
        {
            if (scrollRect.IsScrollingNeeded())
            {
                scroller.SetScrollPosition(1 - scrollValue);
            }
        }
    }
}
