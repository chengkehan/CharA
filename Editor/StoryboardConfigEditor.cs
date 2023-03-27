#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameScript;
using GameScript.Cutscene;
using StoryboardCore;
using XNodeEditor;

namespace GameScriptEditor
{
    [CustomEditor(typeof(StoryboardConfig))]
    public class StoryboardConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var storyboardConfig = target as StoryboardConfig;

            if (GUILayout.Button("Open Storyboard"))
            {
                bool opened = false;

                var assetIds = AssetDatabase.FindAssets("t:Storyboard");
                foreach (var assetId in assetIds)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(assetId);
                    var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                    if (asset.name == storyboardConfig.storyboardName)
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
