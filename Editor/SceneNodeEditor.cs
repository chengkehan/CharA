#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameScript;
using GameScript.Cutscene;

namespace GameScriptEditor
{
    [CustomEditor(typeof(SceneNode))]
    public class SceneNodeEditor : Editor
    {
        private Object buildingAsset = null;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);

            var sceneNode = target as SceneNode;
            var buildingAssetSO = serializedObject.FindProperty("_buildingAssetPath");

            EditorGUI.BeginChangeCheck();
            buildingAsset = EditorGUILayout.ObjectField(buildingAsset, typeof(Object), false);
            if (EditorGUI.EndChangeCheck())
            {
                buildingAssetSO.stringValue = AssetDatabase.GetAssetPath(buildingAsset);
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Asset:", GUILayout.Width(40));
                EditorGUILayout.LabelField(buildingAssetSO.stringValue);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Preview Load/Unload"))
            {
                if (sceneNode.HasSceneGameObject())
                {
                    var go = sceneNode.GetSceneGameObject();
                    GameObject.DestroyImmediate(go);
                    EditorUtility.SetDirty(sceneNode);
                }
                else
                {
                    var asset = AssetDatabase.LoadMainAssetAtPath(buildingAssetSO.stringValue);
                    if (asset != null)
                    {
                        var go = PrefabUtility.InstantiatePrefab(asset) as GameObject;
                        sceneNode.SetSceneGameObject(go);
                        EditorUtility.SetDirty(sceneNode);
                    }
                }
            }
            if (GUILayout.Button("Select Prefab"))
            {
                var asset = AssetDatabase.LoadMainAssetAtPath(buildingAssetSO.stringValue);
                Selection.activeObject = asset;
            }
        }
    }
}
#endif
