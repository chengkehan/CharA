#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;
using GameScript;

namespace GameScriptEditor
{
    public class GameAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var importedAsset in importedAssets)
            {
                if (CheckForIncludeShader(importedAsset))
                {
                    ProcessIncludeShader();
                }
                if (IsJcShader(importedAsset))
                {
                    RegisterJcShader(importedAsset);
                }
            }

            {
                bool excelImported = false;
                foreach (var importedAsset in importedAssets)
                {
                    if (importedAsset.EndsWith("xlsx"))
                    {
                        excelImported = true;
                    }
                }
                if (excelImported)
                {
                    ExcelConfigsEditor.Execute();
                }
            }
        }

        public const string COMMENT = "// ";
        public const string INCLUDE_SHADER_START = "INCLUDE SHADER";
        public const string INCLUDE_SHADER_PATH = "Assets/GameRes/Shader";
        public const string INCLUDE_SHADER_FLAG1 = "// The following code is generated automatically, please change it in the template.VVVVVVVVVVVVVVVVVVVVVVVVVVVVVV";
        public const string INCLUDE_SHADER_FLAG2 = "// The above code is generated automatically, please change it in the template.^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^";
        public const string JC_SHADER = "jcshader";
        public const string DEFINE_ASSET = "asset";
        public const string IF_DEFINE_PREFIX = "IF_DEFINED";
        public const string IF_DEFINE_SUFFIX = "END_IF";
        private static bool IsJcShader(string importedAsset)
        {
            return importedAsset.EndsWith("." + JC_SHADER);
        }
        private static bool IsHLSL(string importedAsset)
        {
            return importedAsset.EndsWith(".hlsl");
        }
        private static bool IsTxt(string importedAsset)
        {
            return importedAsset.EndsWith(".txt");
        }
        private static void RegisterJcShader(string importedAsset)
        {
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(importedAsset);
            if (shader != null)
            {
                ShaderUtil.RegisterShader(shader);
                ShaderUtil.ClearShaderMessages(shader);
            }
        }
        private static bool CheckForIncludeShader(string importedAsset)
        {
            if (importedAsset.Contains(INCLUDE_SHADER_PATH) && (IsTxt(importedAsset) || IsHLSL(importedAsset)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static void ProcessIncludeShader()
        {
            string[] files = Directory.GetFiles(INCLUDE_SHADER_PATH);
            foreach (var file in files)
            {
                if (IsJcShader(file))
                {
                    AssetDatabase.ImportAsset(file);
                }
            }
        }
    }
}
#endif