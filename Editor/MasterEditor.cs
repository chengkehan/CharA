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
                            EditorUtility.RevealInFinder(DataCenter.SAVE_DIRECTORY);
                        }
                        else
                        {
                            EditorUtility.RevealInFinder(DataCenter.SAVE_PATH);
                        }
                    }
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