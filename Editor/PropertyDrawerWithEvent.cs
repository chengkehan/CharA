#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameScriptEditor
{
    public abstract class PropertyDrawerWithEvent : PropertyDrawer
    {
        bool init = true;

        ~PropertyDrawerWithEvent()
        {
            Destroy();
        }

        private void PlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    Destroy();
                    break;
            }
        }

        private void SelectionChanged()
        {
            Disable();
        }

        /// <summary>
        /// Write code for when the property is first displayed or redisplayed.
        /// </summary>
        public abstract void OnEnable(Rect position, SerializedProperty property, GUIContent label);

        /// <summary>
        /// Write code for when the property may be hidden.
        /// </summary>
        public abstract void OnDisable();

        /// <summary>
        /// Write code for when the property is destroyed. (e.g. Releasing resources.)
        /// </summary>
        public abstract void OnDestroy();

        public abstract void OnGUIWithEvent(Rect position, SerializedProperty property, GUIContent label);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (init) Enable(position, property, label);

            OnGUIWithEvent(position, property, label);
        }

        public void Enable(Rect position, SerializedProperty property, GUIContent label)
        {
            init = false;
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
            Selection.selectionChanged += SelectionChanged;
            OnEnable(position, property, label);
        }

        public void Disable()
        {
            OnDisable();
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
            Selection.selectionChanged -= SelectionChanged;
            init = true;
        }

        public void Destroy()
        {
            OnDestroy();
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
            Selection.selectionChanged -= SelectionChanged;
            init = true;
        }
    }
}
#endif
