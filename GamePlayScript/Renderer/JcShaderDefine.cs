using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using GameScriptEditor;
#endif

namespace GameScript
{
    [System.Serializable]
    public class JcShaderDefine : ScriptableObject
    {
        public string[] defines = null;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(JcShaderDefine))]
    public class JcShaderDefineInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Apply"))
            {
                string assetPath = AssetDatabase.GetAssetPath(target);
                assetPath = assetPath.Replace(GameAssetPostprocessor.DEFINE_ASSET, GameAssetPostprocessor.JC_SHADER);
                if (File.Exists(assetPath))
                {
                    AssetDatabase.ImportAsset(assetPath);
                }
            }
        }
    }

#endif
}