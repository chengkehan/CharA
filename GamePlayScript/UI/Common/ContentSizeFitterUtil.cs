using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace GameScript.UI.Common
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class ContentSizeFitterUtil : ContentSizeFitter
    {
        [NonSerialized]
        private RectTransform m_Rect;

        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                {
                    m_Rect = GetComponent<RectTransform>();
                }

                return m_Rect;
            }
        }

        [SerializeField]
        private float m_MaxWidth = -1;

        public float maxWidth
        {
            get => m_MaxWidth;
            set => m_MaxWidth = value;
        }

        [SerializeField]
        private float m_MaxHeight = -1;

        public float maxHeight
        {
            get => m_MaxHeight;
            set => m_MaxHeight = value;
        }

        public override void SetLayoutHorizontal()
        {
            base.SetLayoutHorizontal();

            if (maxWidth > 0)
            {
                if (horizontalFit == FitMode.MinSize)
                {
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Min(LayoutUtility.GetMinSize(m_Rect, 0), maxWidth));
                }
                else if (horizontalFit == FitMode.PreferredSize)
                {
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Min(LayoutUtility.GetPreferredSize(m_Rect, 0), maxWidth));
                }
            }
        }

        public override void SetLayoutVertical()
        {
            base.SetLayoutVertical();

            if (maxHeight > 0)
            {
                if (verticalFit == FitMode.MinSize)
                {
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Min(LayoutUtility.GetMinSize(m_Rect, 1), maxHeight));
                }
                else if (verticalFit == FitMode.PreferredSize)
                {
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Min(LayoutUtility.GetPreferredSize(m_Rect, 1), maxHeight));
                }
            }
        }
    }
}

#if UNITY_EDITOR
namespace GameScript.UI.Common
{
    [CustomEditor(typeof(ContentSizeFitterUtil), true)]
    [CanEditMultipleObjects]
    public class ContentSizeFitterUtilEditor : ContentSizeFitterEditor
    {
        SerializedProperty m_MaxWidth;

        SerializedProperty m_MaxHeight;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_MaxWidth = serializedObject.FindProperty("m_MaxWidth");
            m_MaxHeight = serializedObject.FindProperty("m_MaxHeight");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_MaxWidth, true);
            EditorGUILayout.PropertyField(m_MaxHeight, true);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
#endif