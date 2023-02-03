#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameScript;
using StoryboardCore;
using XNodeEditor;

namespace GameScriptEditor
{
    [CustomEditor(typeof(RoleTalkConfig))]
    public class RoleTalkConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var roleTalkConfig = target as RoleTalkConfig;

            if (GUILayout.Button("Open Storyboard"))
            {
                bool opened = false;

                var assetIds = AssetDatabase.FindAssets("t:Storyboard");
                foreach (var assetId in assetIds)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(assetId);
                    var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                    if (asset.name == roleTalkConfig.storyboardName)
                    {
                        opened = true;
                        NodeEditorWindow.Open(asset as Storyboard);
                        break;
                    }
                }

                if (opened == false)
                {
                    EditorUtility.DisplayDialog(string.Empty, "Storyboard not found.", "ok");
                }
            }
        }
    }
}
#endif
