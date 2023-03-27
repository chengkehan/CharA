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
        string wrapFlag = "Auto Generate Wrap";
        bool startReadConfig = false;
        var configs = new List<string>();

        var allLines = new List<string>(File.ReadLines("Assets/GameRes/GamePlayScript/Data/PlayerData.cs"));
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
                        Utils.Log(configs[configs.Count - 1]);
                    }
                }
            }
            else
            {
                if (line.Contains(wrapFlag))
                {
                    

                }
            }
        }
    }
}
#endif
