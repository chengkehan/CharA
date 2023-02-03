using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScript.UI.Common
{
    public class Scroller : ComponentBase
    {
        public ScrollerThumb thumb = null;

        public ScrollerTrack track = null;

        public Transform topLimitingStopper = null;

        public Transform bottomLimitingStopper = null;

        private bool _isthumbPointerDown = false;

        private Vector2 _thumbPointerDownOffset = Vector2.zero;

        private Action<float> _scrollValueChangedCB = null;

        public void SetScrollValueChangedCB(Action<float> cb)
        {
            _scrollValueChangedCB = cb;
        }

        public Action<float> GetScrollValueChangedCB()
        {
            return _scrollValueChangedCB;
        }

        public float GetScrollPosition()
        {
            float minY = GetBottomLimitingStopperY();
            float maxY = GetTopLimitingStopperY();
            float currentY = thumb.GetAnchoredPoint().y;
            return 1 - Mathf.Clamp01((currentY - minY) / (maxY - minY));
        }

        public void SetScrollPosition(float v)
        {
            v = 1 - Mathf.Clamp01(v);
            float minY = GetBottomLimitingStopperY();
            float maxY = GetTopLimitingStopperY();
            float currentY = minY + (maxY - minY) * v;

            Vector2 anchoredPoint = thumb.GetAnchoredPoint();
            anchoredPoint.y = currentY;
            thumb.SetAnchoredPoint(anchoredPoint);
        }

        protected override void Awake()
        {
            base.Awake();

            thumb.SetOnPointerDownCB(ThumbOnPointerDownCB);
            thumb.SetOnPointerUpCB(ThumbOnPointerUpCB);
        }

        private void ThumbOnPointerUpCB()
        {
            SetIsThumbPointerDown(false);
        }

        private void ThumbOnPointerDownCB()
        {
            SetIsThumbPointerDown(true);
        }

        private void Update()
        {
            UpdateThumbPositionWhenIsThumbPointerDown();
        }

        private void UpdateThumbPositionWhenIsThumbPointerDown()
        {
            if (GetIsThumbPointerDown())
            {
                if (ScreenPointToLocalPointInRectangle(
                    GetRectTransform(), Input.mousePosition, out Vector2 localPoint))
                {
                    Vector2 anchoredPoint = thumb.GetAnchoredPoint();
                    anchoredPoint.y = localPoint.y;
                    anchoredPoint.y -= GetThumbPointerDownOffset().y;
                    anchoredPoint.y = Mathf.Clamp(anchoredPoint.y, GetBottomLimitingStopperY(), GetTopLimitingStopperY());
                    thumb.SetAnchoredPoint(anchoredPoint);

                    GetScrollValueChangedCB()?.Invoke(GetScrollPosition());
                }
            }
        }

        private void SetIsThumbPointerDown(bool b)
        {
            _isthumbPointerDown = b;
            if (_isthumbPointerDown)
            {
                CalculateThumbPointerDownOffset();
            }
        }

        private bool GetIsThumbPointerDown()
        {
            return _isthumbPointerDown;
        }

        private void CalculateThumbPointerDownOffset()
        {
            if (ScreenPointToLocalPointInRectangle(
                GetRectTransform(), Input.mousePosition, out Vector2 localPoint))
            {
                _thumbPointerDownOffset = localPoint - thumb.GetAnchoredPoint();
            }
        }

        private Vector2 GetThumbPointerDownOffset()
        {
            return _thumbPointerDownOffset;
        }

        private float GetTopLimitingStopperY()
        {
            if (ConvertWorldPositionToLocalPoint(
                topLimitingStopper.position, false, GetRectTransform(), out Vector2 localPoint))
            {
                return localPoint.y;
            }
            else
            {
                return 0;
            }
        }

        private float GetBottomLimitingStopperY()
        {
            if (ConvertWorldPositionToLocalPoint(
                bottomLimitingStopper.position, false, GetRectTransform(), out Vector2 localPoint))
            {
                return localPoint.y;
            }
            else
            {
                return 0;
            }
        }
    }
}
