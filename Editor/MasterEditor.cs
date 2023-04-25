#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using GameScript;

namespace GameScriptEditor
{
    public class MasterEditor : EditorWindow
    {
        [MenuItem("GameTools/MasterEditor")]
        private static void OpenMasterEditor()
        {
            EditorWindow.GetWindow<MasterEditor>().Show();
        }

        public static Object GetStoryboardAsset(string storyboardName)
        {
            var assetIds = AssetDatabase.FindAssets("t:Storyboard");
            foreach (var assetId in assetIds)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetId);
                var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                if (asset.name == storyboardName)
                {
                    return asset;
                }
            }

            return null;
        }

        public static void Select(string guid)
        {
            bool found = false;
            var objs = FindObjectsOfType<GuidMonoBehaviour>();
            foreach (var obj in objs)
            {
                if (obj.guid == guid)
                {
                    Selection.activeGameObject = obj.gameObject;
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                Utils.Log("Cannot find " + guid + " in scene.");
            }
        }

        private string guidInput = string.Empty;

        private void OnGUI()
        {
            BeginBox();
            {
                if (Application.isPlaying == false)
                {
                    EditorGUILayout.HelpBox("Must be playing", MessageType.None);
                }
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Save Playing User Data");
                    GUI.enabled = Application.isPlaying;
                    if (GUILayout.Button("Save"))
                    {
                        DataCenter.GetInstance().Save();
                        ShowNotification(new GUIContent("Saved"));
                    }
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            EndBox();

            BeginBox();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Open Saved Folder");
                    if (GUILayout.Button("Open"))
                    {
                        if (File.Exists(DataCenter.SAVE_PATH) == false)
                        {
                            Debug.Log(DataCenter.SAVE_DIRECTORY);
                            EditorUtility.RevealInFinder(DataCenter.SAVE_DIRECTORY);
                        }
                        else
                        {
                            Debug.Log(DataCenter.SAVE_PATH);
                            EditorUtility.RevealInFinder(DataCenter.SAVE_PATH);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EndBox();

            BeginBox();
            {
                guidInput = EditorGUILayout.TextField(guidInput);
                if (GUILayout.Button("Find In Scene By GUID"))
                {
                    Select(guidInput);
                }

            }
            EndBox();
        }

        private void BeginBox()
        {
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("Box"));
        }

        private void EndBox()
        {
            EditorGUILayout.EndVertical();
        }
    }
}
#endif