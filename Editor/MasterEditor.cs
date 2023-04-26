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

        #region Jump Back

        private static Object _jumpBackTarget = null;

        public static void RecordSelection()
        {
            _jumpBackTarget = Selection.activeObject;
        }

        public static void JumpBackSelection()
        {
            if (_jumpBackTarget != null)
            {
                Selection.activeObject = _jumpBackTarget;
                _jumpBackTarget = null;
            }
        }

        public static bool HasRecordedSelection()
        {
            return _jumpBackTarget != null;
        }

        #endregion

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

        public static T Find<T>(string guid)
            where T : GuidMonoBehaviour
        {
            var objs = FindObjectsOfType<T>();
            foreach (var obj in objs)
            {
                if (obj.guid == guid)
                {
                    return obj;
                }
            }
            return null;
        }

        public static void Select<T>(string guid)
            where T : GuidMonoBehaviour
        {
            T obj = Find<T>(guid);
            if (obj == null)
            {
                Utils.Log("Cannot find " + guid + " in scene.");
            }
            else
            {
                Selection.activeGameObject = obj.gameObject;
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
                    Select<GuidMonoBehaviour>(guidInput);
                }

            }
            EndBox();

            BeginBox();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Jump Back");
                    GUI.enabled = HasRecordedSelection();
                    if (GUILayout.Button("->"))
                    {
                        JumpBackSelection();
                    }
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
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