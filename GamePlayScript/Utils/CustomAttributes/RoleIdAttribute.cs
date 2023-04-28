using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameScript
{
    public class RoleIdAttribute : PropertyAttribute
    {
        
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(RoleIdAttribute))]
    public class RoleIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float buttonSize = 20;
            float gap = 2;

            Rect newPosition = position;
            newPosition.width -= buttonSize + gap;
            EditorGUI.PropertyField(newPosition, property, label);

            string roleId = property.stringValue;

            Rect buttonPosition = newPosition;
            buttonPosition.x += buttonPosition.width + gap;
            buttonPosition.width = buttonSize;
            var roleHeadIcon = AssetDatabase.LoadMainAssetAtPath(AssetsManager.GetEditorHeadIconPath(roleId)) as Texture;
            if (roleHeadIcon != null)
            {
                GUI.DrawTexture(buttonPosition, roleHeadIcon, ScaleMode.ScaleToFit);
            }
            else
            {
                Utils.guiColor.Record();
                {
                    Utils.guiColor.Alert();
                    GUI.DrawTexture(buttonPosition, Texture2D.whiteTexture, ScaleMode.StretchToFill);
                }
                Utils.guiColor.Reset();
            }
        }
    }
#endif
}
