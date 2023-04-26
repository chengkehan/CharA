using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using GameScriptEditor;
#endif

namespace GameScript
{
    public class GuidMonoAttribute : PropertyAttribute
    {
        
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GuidMonoAttribute))]
    public class GuidMonoDrawer : PropertyDrawer
    {
        private GUIContent findButtonGUIContent = new GUIContent("F", "Find GuidMono in Scene.");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float buttonSize = 20;
            float gap = 2;

            Rect newPosition = position;
            newPosition.width -= buttonSize + gap;
            EditorGUI.PropertyField(newPosition, property, label);

            Rect buttonPosition = newPosition;
            buttonPosition.x += buttonPosition.width + gap;
            buttonPosition.width = buttonSize;
            if (GUI.Button(buttonPosition, findButtonGUIContent))
            {
                MasterEditor.RecordSelection();
                MasterEditor.Select<GuidMonoBehaviour>(property.stringValue);
            }
        }
    }
#endif
}
