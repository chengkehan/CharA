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

        private GUIContent findButtonGUIContent = new GUIContent("F", "Find Storyboard");

        private string input = null;

        private Object asset = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float buttonSize = 20;
            float gap = 2;

            Rect newPosition = position;
            newPosition.width -= buttonSize * 2 + gap * 2;
            EditorGUI.PropertyField(newPosition, property, label);

            if (property.stringValue != input)
            {
                input = property.stringValue;
                asset = MasterEditor.GetStoryboardAsset(property.stringValue);
            }

            Rect openButtonPosition = newPosition;
            openButtonPosition.x += openButtonPosition.width + gap;
            openButtonPosition.width = buttonSize;

            Utils.guiColor.Record();
            {
                if (asset == null)
                {
                    Utils.guiColor.Alert();
                }

                if (GUI.Button(openButtonPosition, openButtonGUIContent))
                {
                    if (asset != null)
                    {
                        NodeEditorWindow.Open(asset as Storyboard);
                    }
                    else
                    {
                        Utils.Log("Cannot find Storyboard. " + property.stringValue);
                    }
                }

                Rect findButtonPosition = openButtonPosition;
                findButtonPosition.x += buttonSize + gap;
                if (GUI.Button(findButtonPosition, findButtonGUIContent))
                {
                    if (asset != null)
                    {
                        MasterEditor.RecordSelection();
                        Selection.activeObject = asset;
                    }
                    else
                    {
                        Utils.Log("Cannot find Storyboard. " + property.stringValue);
                    }
                }
            }
            Utils.guiColor.Reset();
        }
    }
#endif
}
