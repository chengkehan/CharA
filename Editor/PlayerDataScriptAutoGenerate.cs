#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using GameScript;

public class PlayerDataScriptAutoGenerate
{
    [MenuItem("GameTools/PlayerData Script AutoGenerate")]
    private static void Execute()
    {
        string configFlag = "Auto Generate Config";
        string fieldsFlag = "Auto Generate Fields";
        string wrapFlag = "Auto Generate Wrap";
        string savingFlag = "Auto Generate Saving";
        bool startReadConfig = false;
        var configs = new List<string>();

        string filePath = "Assets/GameRes/GamePlayScript/Data/PlayerData.cs";
        var allLines = new List<string>(File.ReadLines(filePath));
        for (int lineI = 0; lineI < allLines.Count; lineI++)
        {
            var line = allLines[lineI];

            if (line.Contains(configFlag))
            {
                if (startReadConfig == false)
                {
                    startReadConfig = true;
                    continue;
                }
                else
                {
                    startReadConfig = false;
                }
            }

            if (startReadConfig)
            {
                var lineStr = line.Trim();
                if (string.IsNullOrEmpty(lineStr) == false)
                {
                    var blocks = lineStr.Split('|');
                    for (int blockI = 0; blockI < blocks.Length; blockI++)
                    {
                        configs.Add(blocks[blockI].Trim());
                    }
                }
            }
            else
            {
                lineI = GenerateScript(allLines, configs, fieldsFlag, line, lineI,
                    "[SerializeField] private SerializableDictionary<string, {x}PD> all{x}PD = new SerializableDictionary<string, {x}PD>();");

                lineI = GenerateScript(allLines, configs, wrapFlag, line, lineI,
                    "allPDObjects.Add(new PDObjects() { pdType = typeof({x}PD), monoType = typeof({x}), collection = all{x}PD });");

                lineI = GenerateScript(allLines, configs, savingFlag, line, lineI,
                    "SaveSerializableMonoBehaviours<{x}PD, {x}>(all{x}PD);");
            }
        }

        File.WriteAllLines(filePath, allLines);
        AssetDatabase.ImportAsset(filePath);
    }

    private static int GenerateScript(List<string> allLines, List<string> configs, string flag, string line, int lineI, string script)
    {
        if (line.Contains(flag))
        {
            for (int i = lineI + 1; i < allLines.Count; i++)
            {
                if (allLines[i].Contains(flag) == false)
                {
                    allLines.RemoveAt(i);
                    --i;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < configs.Count; i++)
            {
                allLines.Insert(lineI + 1 + i, script.Replace("{x}", configs[i]));
            }

            lineI += configs.Count + 1;
            return lineI;
        }
        else
        {
            return lineI;
        }
    }
}
#endif
