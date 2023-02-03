#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using StoryboardCore;
using GameScript;

namespace StoryboardEditor
{
    public class NodeEditorBase : NodeEditor
    {
        #region Warning Information

        protected const string WARNING_TEXT_LINK_NEXT_PORT = "Next port should be linked.";
        protected const string WARNING_TEXT_LINK_PREV_PORT = "Prev port should be linked.";

        private string warningText = string.Empty;

        public bool HasWarningInformation()
        {
            return string.IsNullOrEmpty(warningText) == false;
        }

        private void AppendWarningText(string text)
        {
            if (string.IsNullOrWhiteSpace(warningText))
            {
                warningText += text;
            }
            else
            {
                warningText += "\n" + text;
            }
        }

        protected void Validate(bool aNewValidation, bool b1, string s1, bool b2 = true, string s2 = null, bool b3 = true, string s3 = null, bool b4 = true, string s4 = null, bool b5 = true, string s5 = null)
        {
            if (aNewValidation)
            {
                warningText = string.Empty;
            }

            if (b1 == false && string.IsNullOrWhiteSpace(s1) == false)
            {
                AppendWarningText(s1);
            }
            if (b2 == false && string.IsNullOrWhiteSpace(s2) == false)
            {
                AppendWarningText(s2);
            }
            if (b3 == false && string.IsNullOrWhiteSpace(s3) == false)
            {
                AppendWarningText(s3);
            }
            if (b4 == false && string.IsNullOrWhiteSpace(s4) == false)
            {
                AppendWarningText(s4);
            }
            if (b5 == false && string.IsNullOrWhiteSpace(s5) == false)
            {
                AppendWarningText(s5);
            }
        }

        public override Texture LeftTopIcon()
        {
            return string.IsNullOrWhiteSpace(warningText) ? null : EditorTextures.storyboard_warning;
        }

        public override string LeftTopIconTooltip()
        {
            return warningText;
        }

        #endregion

        #region Description GUI Label

        private GUIStyle infoLabelStyle = null;

        protected void DrawInfoLabel(string txt)
        {
            if (string.IsNullOrWhiteSpace(txt) == false)
            {
                if (infoLabelStyle == null)
                {
                    infoLabelStyle = new GUIStyle();
                    infoLabelStyle.fontSize = 20;
                    infoLabelStyle.normal.textColor = Color.white;
                    infoLabelStyle.wordWrap = true;
                }
                GUILayout.Label(txt, infoLabelStyle);
            }
        }

        #endregion

        #region Tips GUI Label

        private GUIStyle tipsLabelStyle = null;

        private GUIContent tipsGUIContent = null;

        protected void DrawTipsLabel(string txt, string tooltip)
        {
            if (string.IsNullOrWhiteSpace(txt) == false)
            {
                if (tipsLabelStyle == null)
                {
                    tipsLabelStyle = new GUIStyle();
                    tipsLabelStyle.fontSize = 12;
                    tipsLabelStyle.normal.textColor = Color.gray;
                    tipsLabelStyle.wordWrap = true;
                }
                if (tipsGUIContent == null)
                {
                    tipsGUIContent = new GUIContent();
                }

                tipsGUIContent.text = txt;
                tipsGUIContent.tooltip = tooltip;
                GUILayout.Label(tipsGUIContent, tipsLabelStyle);
            }
        }

        #endregion

        #region Role head icon

        protected void DrawRoleHeadIcon(string roleId)
        {
            var roleHeadIcon = AssetDatabase.LoadMainAssetAtPath(AssetsManager.EDITOR_ROLE_HEAD_ICON_PATH + roleId + ".png") as Texture;
            if (roleHeadIcon != null)
            {
                Rect rect = GUILayoutUtility.GetRect(50, 50);
                GUI.DrawTexture(rect, roleHeadIcon, ScaleMode.ScaleToFit);
            }
        }

        #endregion
    }
}
#endif
