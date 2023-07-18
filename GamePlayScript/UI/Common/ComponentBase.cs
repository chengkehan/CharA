using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoryboardCore;
using System;

namespace GameScript.UI.Common
{
    public class ComponentBase : MonoBehaviour
    {
        #region RectTransform

        private RectTransform _rectTransform = null;

        public RectTransform GetRectTransform()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }

        #endregion

        public void SetVisible(bool visible)
        {
            if (gameObject != null && gameObject.activeSelf != visible)
            {
                gameObject.SetActive(visible);
            }
        }

        public bool GetVisible()
        {
            return gameObject == null ? false : gameObject.activeSelf;
        }

        public void SetAnchoredPoint(Vector2 pos)
        {
            if (GetRectTransform() != null)
            {
                GetRectTransform().anchoredPosition = pos;
            }
        }

        public Vector2 GetAnchoredPoint()
        {
            if (GetRectTransform() != null)
            {
                return GetRectTransform().anchoredPosition;
            }
            else
            {
                return Vector2.zero;
            }
        }

        public bool InsertAfter(ComponentBase target)
        {
            if (target == null || GetRectTransform() == null || target.GetRectTransform() == null)
            {
                return false;
            }

            GetRectTransform().SetSiblingIndex(target.GetRectTransform().GetSiblingIndex() + 1);
            return true;
        }

        public bool InsertBefore(ComponentBase target)
        {
            if (target == null || GetRectTransform() == null || target.GetRectTransform() == null)
            {
                return false;
            }

            GetRectTransform().SetSiblingIndex(target.GetRectTransform().GetSiblingIndex());
            return true;
        }

        public static bool ScreenPointToLocalPointInRectangle(RectTransform rectTransform, Vector2 screenPoint, out Vector2 localPoint)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, screenPoint, CameraManager.GetInstance().GetUICamera(), out Vector2 lp))
            {
                localPoint = lp;
                return true;
            }
            else
            {
                localPoint = Vector2.zero;
                return false;
            }
        }

        public static bool ConvertWorldPositionToLocalPoint(Vector3 wPos, bool wPosIs3D, RectTransform rectTransform, out Vector2 localPoint)
        {
            var cam = wPosIs3D ? CameraManager.GetInstance().GetMainCamera() : CameraManager.GetInstance().GetUICamera();
            if (cam == null)
            {
                localPoint = Vector2.zero;
                return false;
            }
            else
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, wPos);
                if (ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, out Vector2 lp))
                {
                    localPoint = lp;
                    return true;
                }
                else
                {
                    localPoint = Vector2.zero;
                    return false;
                }
            }
        }

        protected virtual void Awake()
        {
            AddListeners_UGUI_EventSystem();
        }

        protected virtual void Start()
        {
            // Do nothing
        }

        protected virtual void OnDestroy()
        {
            DestroyTooltip();
        }

        #region UGUI EventSystem Changed

        private void AddListeners_UGUI_EventSystem()
        {
            if (tooltipEnabled)
            {
                EventSystem.GetInstance().AddListener(EventID.UGUI_EventSystemChanged, UGUI_EventSystemChangedHandler, gameObject);
            }
        }

        private void UGUI_EventSystemChangedHandler(NotificationData _data)
        {
            var data = _data as UGUI_EventSystemChangedND;
            if (data != null)
            {
                OnUGUIEventSystemChanged(data.enabled);
            }
        }

        protected virtual void OnUGUIEventSystemChanged(bool enabled)
        {
            HideTooltip();
        }

        #endregion

        #region Language

        public string GetLanguage(string key)
        {
            var t = transform;
            while (t != null)
            {
                var uiBase = t.GetComponent<UIBase>();
                if (uiBase != null)
                {
                    return uiBase.GetLanguage(key);
                }

                t = t.parent;
            }

            return string.Empty;
        }

        #endregion

        #region Tooltip

        [SerializeField]
        private bool _tooltipEnabled = false;
        private bool tooltipEnabled
        {
            get
            {
                return _tooltipEnabled;
            }
        }

        [ConditionDisable("_tooltipEnabled", true)]
        [SerializeField]
        [Range(-100, 100)]
        private float _tooltipX = 0;

        [ConditionDisable("_tooltipEnabled", true)]
        [SerializeField]
        [Range(-100, 100)]
        private float _tooltipY = 0;

        [ConditionDisable("_tooltipEnabled", true)]
        [SerializeField]
        private RectTransform _tooltipParent = null;

        [ConditionDisable("_tooltipEnabled", true)]
        [SerializeField]
        private bool _pushToTopWhenTooltip = false; 

        private bool _tooltipVisible = false;

        private Tooltip _tooltip = null;

        private bool _tooltipIsLoading = false;

        protected void ShowTooltip(string txt, Action discardHandler = null, Action eatHandler = null, Action transferHandler = null)
        {
            if (_tooltipEnabled == false)
            {
                return;
            }

            _tooltipVisible = true;

            if (_tooltip == null)
            {
                if (_tooltipIsLoading == false)
                {
                    _tooltipIsLoading = true;
                    AssetsManager.GetInstance().LoadGameObject(AssetsManager.UI_ASSET_PREFIX + "Tooltip", (obj) =>
                    {
                        _tooltipIsLoading = false;
                        _tooltip = obj.GetComponent<Tooltip>();
                        _tooltip.tooltip_text_discard_eat_transfer.Set(txt, eatHandler, discardHandler, transferHandler);
                        _tooltip.SetVisible(_tooltipVisible);

                        Vector3 wPos = GetRectTransform().localToWorldMatrix.MultiplyPoint(new Vector3(_tooltipX, _tooltipY, 0));

                        var tooltipParent = _tooltipParent == null ? GetRectTransform() : _tooltipParent;

                        var tooltipTransform = _tooltip.GetComponent<RectTransform>();
                        tooltipTransform.SetParent(tooltipParent, false);
                        if (ConvertWorldPositionToLocalPoint(wPos, false, tooltipParent, out var localPoint))
                        {
                            tooltipTransform.anchoredPosition = localPoint;
                        }

                        TryPushToTopWhenTooltip();
                    });
                }
            }
            else
            {
                _tooltip.tooltip_text_discard_eat_transfer.Set(txt, eatHandler, discardHandler, transferHandler);
                _tooltip.SetVisible(_tooltipVisible);
                TryPushToTopWhenTooltip();
            }
        }

        protected void HideTooltip()
        {
            if (_tooltipEnabled == false)
            {
                return;
            }

            _tooltipVisible = false;
            if (_tooltip != null)
            {
                _tooltip.SetVisible(_tooltipVisible);
            }
        }

        private void DestroyTooltip()
        {
            if (_tooltip != null)
            {
                AssetsManager.GetInstance().UnloadGameObject(_tooltip.gameObject);
                _tooltip = null;
            }
        }

        private void TryPushToTopWhenTooltip()
        {
            if (_pushToTopWhenTooltip)
            {
                if (GetRectTransform() != null)
                {
                    GetRectTransform().SetAsLastSibling();
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            {
                Gizmos.DrawSphere(new Vector3(_tooltipX, _tooltipY, 0), 3f);
            }
            Gizmos.matrix = Matrix4x4.identity;
        }
#endif

#endregion
    }
}