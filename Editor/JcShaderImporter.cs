#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using GameScript;

namespace GameScriptEditor
{
    [ScriptedImporter(1, GameAssetPostprocessor.JC_SHADER)]
    public class JcShaderImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string content = GenerateCode(ctx.assetPath);
            Shader shader = ShaderUtil.CreateShaderAsset(content);
            ctx.AddObjectToAsset("MainObject", shader);
            ctx.SetMainObject(shader);
        }

        public static string GenerateCode(string assetPath)
        {
            string[] lines = File.ReadAllLines(assetPath);
            string[] newLines = lines;
            for (int i = 0; i < 3/*maximum iteration depth*/; i++)
            {
                newLines = Parse(newLines, assetPath);
            }
            string content = string.Join("\n", newLines);
            return content;
        }

        private static string[] Parse(string[] lines, string assetPath)
        {
            List<string> newLines = new List<string>(lines);
            int numLines = newLines.Count;
            List<string> pendingIncludeLines = null;

            int UNKNOWN = -1;
            int UNDEFINED = 0;
            int DEFINED = 1;
            int isDefined = UNKNOWN;

            for (int i = 0; i < numLines; i++)
            {
                string currentLine = newLines[i];
                if (isDefined == UNKNOWN && currentLine.Contains(GameAssetPostprocessor.IF_DEFINE_PREFIX) && !currentLine.StartsWith(GameAssetPostprocessor.COMMENT))
                {
                    string definition = currentLine.Substring(currentLine.IndexOf(GameAssetPostprocessor.IF_DEFINE_PREFIX) + GameAssetPostprocessor.IF_DEFINE_PREFIX.Length);
                    definition = definition.Trim();
                    string defineAssetPath = assetPath.Replace(GameAssetPostprocessor.JC_SHADER, GameAssetPostprocessor.DEFINE_ASSET);

                    newLines[i] = GameAssetPostprocessor.COMMENT + newLines[i];

                    JcShaderDefine jcsd = AssetDatabase.LoadMainAssetAtPath(defineAssetPath) as JcShaderDefine;
                    if (jcsd != null && jcsd.defines != null && System.Array.IndexOf(jcsd.defines, definition) != -1)
                    {
                        isDefined = DEFINED;
                    }
                    else
                    {
                        isDefined = UNDEFINED;
                    }
                }
                if (isDefined != UNKNOWN)
                {
                    if (isDefined == UNDEFINED)
                    {
                        newLines[i] = GameAssetPostprocessor.COMMENT + newLines[i];
                    }
                }
                if (isDefined != UNKNOWN && currentLine.Contains(GameAssetPostprocessor.IF_DEFINE_SUFFIX) && !currentLine.StartsWith(GameAssetPostprocessor.COMMENT))
                {
                    newLines[i] = GameAssetPostprocessor.COMMENT + newLines[i];
                    isDefined = UNKNOWN;
                }

                if (isDefined == UNKNOWN)
                {
                    if (currentLine.Contains(GameAssetPostprocessor.INCLUDE_SHADER_START) && !currentLine.StartsWith(GameAssetPostprocessor.COMMENT))
                    {
                        string fileName = currentLine.Substring(currentLine.IndexOf(GameAssetPostprocessor.INCLUDE_SHADER_START) + GameAssetPostprocessor.INCLUDE_SHADER_START.Length);
                        fileName = fileName.Trim();
                        if (File.Exists(GameAssetPostprocessor.INCLUDE_SHADER_PATH + "/" + fileName))
                        {
                            if (pendingIncludeLines == null)
                            {
                                pendingIncludeLines = new List<string>();
                            }
                            pendingIncludeLines.Clear();

                            newLines[i] = GameAssetPostprocessor.COMMENT + newLines[i];
                            pendingIncludeLines.Add(GameAssetPostprocessor.INCLUDE_SHADER_FLAG1);
                            pendingIncludeLines.AddRange(File.ReadAllLines(GameAssetPostprocessor.INCLUDE_SHADER_PATH + "/" + fileName));
                            pendingIncludeLines.Add(GameAssetPostprocessor.INCLUDE_SHADER_FLAG2);

                            newLines.InsertRange(i + 1, pendingIncludeLines);
                            numLines += pendingIncludeLines.Count;
                            i += pendingIncludeLines.Count;
                        }
                        else
                        {
                            Utils.LogError("Include failed. File is not exsited. " + fileName);
                        }
                    }
                }
            }

            return newLines.ToArray();
        }
    }

    [CustomEditor(typeof(JcShaderImporter))]
    public class JcShaderImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Create Define"))
            {
                string assetPath = (serializedObject.targetObject as JcShaderImporter).assetPath;
                assetPath = assetPath.Replace(GameAssetPostprocessor.JC_SHADER, "asset");
                if (File.Exists(assetPath))
                {
                    EditorUtility.DisplayDialog("JcShaderImporter", assetPath + " already exsited.", "ok");
                }
                else
                {
                    JcShaderDefine jcsd = ScriptableObject.CreateInstance<JcShaderDefine>();
                    AssetDatabase.CreateAsset(jcsd, assetPath);
                }
            }
            if (GUILayout.Button("Copy Generated Code"))
            {
                string assetPath = (serializedObject.targetObject as JcShaderImporter).assetPath;
                string content = JcShaderImporter.GenerateCode(assetPath);
                GUIUtility.systemCopyBuffer = content;
                Utils.Log("Code copied.");
            }

            base.ApplyRevertGUI();
        }
    }
}
#endif