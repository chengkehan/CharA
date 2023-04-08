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

        public RectTransform pivot = null;

        public void AdjustPivotPosition()
        {
            if (ActorsManager.GetInstance() != null)
            {
                var heroActor = ActorsManager.GetInstance().GetHeroActor();
                if (heroActor != null)
                {
                    var heroWPos = heroActor.roleAnimation.GetMotionAnimator().GetPosition();
                    var heroFaceTo = heroActor.GetHeadDirection();

                    heroFaceTo.z = 0;
                    heroFaceTo.y = 0;
                    heroFaceTo.Normalize();

                    if (ComponentBase.ConvertWorldPositionToLocalPoint(heroWPos, true, pivot.parent.GetComponent<RectTransform>(), out var localPoint))
                    {
                        //float headIconSize = 120;
                        float halfSize = 336;
                        float offsetX = 100;
                        localPoint.y = pivot.anchoredPosition.y;
                        localPoint.x -= heroFaceTo.x * (halfSize + offsetX);
                        pivot.anchoredPosition = localPoint;

                        if (heroFaceTo.x > 0)
                        {
                            if (ComponentBase.ScreenPointToLocalPointInRectangle(pivot.parent.GetComponent<RectTransform>(), Vector2.zero, out var leftConerLocalPoint))
                            {
                                if (localPoint.x - heroFaceTo.x * halfSize < leftConerLocalPoint.x)
                                {
                                    localPoint.x += 1.8f * heroFaceTo.x * (halfSize + offsetX);
                                    pivot.anchoredPosition = localPoint;
                                }
                            }
                        }
                        else
                        {
                            if (ComponentBase.ScreenPointToLocalPointInRectangle(pivot.parent.GetComponent<RectTransform>(), new Vector2(Screen.width, Screen.height), out var rightConerLocalPoint))
                            {
                                if (localPoint.x - heroFaceTo.x * halfSize > rightConerLocalPoint.x)
                                {
                                    localPoint.x += 1.8f * heroFaceTo.x * (halfSize + offsetX);
                                    pivot.anchoredPosition = localPoint;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddWords(string name, string words, bool isFromChoice, string roleIdForHeadIcon)
        {
            scrollRect.AddWordsItem(name, words, isFromChoice, roleIdForHeadIcon);
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

        protected override void Awake()
        {
            base.Awake();

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
