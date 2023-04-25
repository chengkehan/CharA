using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoryboardCore;
#if UNITY_EDITOR
using UnityEditor;
using GameScriptEditor;
using XNodeEditor;
#endif

namespace GameScript
{
    public class StoryboardNameAttribute : PropertyAttribute
    {
        
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(StoryboardNameAttribute))]
    public class StoryboardNameDrawer : PropertyDrawer
    {
        private GUIContent openButtonGUIContent = new GUIContent("O", "Open Storyboard");

        private GUIContent selectButtonGUIContent = new GUIContent("S", "Select Storyboard");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float buttonSize = 20;
            float gap = 2;

            Rect newPosition = position;
            newPosition.width -= buttonSize * 2 + gap * 2;
            EditorGUI.PropertyField(newPosition, property, label);

            Rect openButtonPosition = newPosition;
            openButtonPosition.x += openButtonPosition.width + gap;
            openButtonPosition.width = buttonSize;
            if (GUI.Button(openButtonPosition, openButtonGUIContent))
            {
                var asset = MasterEditor.GetStoryboardAsset(property.stringValue);
                if (asset != null)
                {
                    NodeEditorWindow.Open(asset as Storyboard);
                }
                else
                {
                    Utils.Log("Cannot find Storyboard. " + property.stringValue);
                }
            }

            Rect selectButtonPosition = openButtonPosition;
            selectButtonPosition.x += buttonSize + gap;
            if (GUI.Button(selectButtonPosition, selectButtonGUIContent))
            {
                var asset = MasterEditor.GetStoryboardAsset(property.stringValue);
                if (asset != null)
                {
                    Selection.activeObject = asset;
                }
                else
                {
                    Utils.Log("Cannot find Storyboard. " + property.stringValue);
                }
            }
        }
    }
#endif
}
