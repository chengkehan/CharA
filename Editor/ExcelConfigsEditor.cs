#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Excel;
using System.Data;
using GameScript;

namespace GameScriptEditor
{
    public class ExcelConfigsEditor
    {
        [MenuItem("GameTools/Config/Convert All Excels")]
        public static void Execute()
        {
            ProcessFolder("Assets/GameRes/Config/", true, true);
            ProcessFolder("Assets/GameRes/Config/StoryboardData/", false, true);
            ProcessFolder("Assets/GameRes/Config/UILanguage/", false, true);
            Utils.Log("Convert All Excels Complete");
        }

        private static void ProcessFolder(string folderPath, bool generateCode, bool generateJson)
        {
            string[] files = Directory.GetFiles(folderPath, "*.xlsx");
            foreach (var file in files)
            {
                if (file.EndsWith(".xlsx"))
                {
                    var fileInfo = new FileInfo(file);
                    var objectName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
                    var className = objectName + "Config";

                    FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read);
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    DataSet result = excelReader.AsDataSet();
                    var table = result.Tables[0];

                    List<string> propertyNames = new List<string>();
                    {
                        var headerRow = table.Rows[0];
                        var headerRowCells = headerRow.ItemArray;
                        foreach (var cell in headerRowCells)
                        {
                            propertyNames.Add(cell.ToString());
                        }
                    }

                    List<string> propertyTypes = new List<string>();
                    {
                        var secondRow = table.Rows[1];
                        var secondRowCells = secondRow.ItemArray;
                        foreach (var cell in secondRowCells)
                        {
                            propertyTypes.Add(cell.ToString());
                        }
                    }

                    Utils.Assert(propertyNames.Count == propertyTypes.Count);

                    if (generateCode)
                    {
                    var qm = "\"";

                    string codeStr =
$@"
using System;
using UnityEngine;

namespace GameScript
{{
    [Serializable]
    public class {className}
    {{
";
                    for (int propertyI = 0; propertyI < propertyNames.Count; propertyI++)
                    {
                        if (string.IsNullOrWhiteSpace(propertyTypes[propertyI]) == false)
                        {
                            var defaultValue = DefaultValueByType(propertyTypes[propertyI]);

                            codeStr +=
$@"
        [SerializeField]
        private {propertyTypes[propertyI]} _{propertyNames[propertyI]} = {defaultValue};
        public {propertyTypes[propertyI]} {propertyNames[propertyI]}
        {{
            get
            {{
                return _{propertyNames[propertyI]};
            }}
        }}
";
                        }
                    }

                    codeStr +=
$@"
        public {className} (";
                    for (int propertyI = 0; propertyI < propertyNames.Count; propertyI++)
                    {
                        if (string.IsNullOrWhiteSpace(propertyTypes[propertyI]) == false)
                        {
                            codeStr += $"{propertyTypes[propertyI]} {propertyNames[propertyI]}, ";
                        }
                    }

                    codeStr = codeStr.Substring(0, codeStr.Length - 2);

                    codeStr += ")";

                    codeStr +=
$@"
        {{";
                    for (int propertyI = 0; propertyI < propertyNames.Count; propertyI++)
                    {
                        if (string.IsNullOrWhiteSpace(propertyTypes[propertyI]) == false)
                        {
                            codeStr +=
$@"
            _{propertyNames[propertyI]} = {propertyNames[propertyI]};";
                        }
                    }

                    codeStr +=
$@"
        }}
";

                    codeStr +=
$@"
        public {className} ()
        {{";

                    for (int propertyI = 0; propertyI < propertyNames.Count; propertyI++)
                    {
                        if (string.IsNullOrWhiteSpace(propertyTypes[propertyI]) == false)
                        {
                            var defaultValue = DefaultValueByType(propertyTypes[propertyI]);

                            codeStr +=
$@"
            _{propertyNames[propertyI]} = {defaultValue};";
                        }
                    }

                    codeStr +=
$@"
        }}";

                    if (className == "LanguageConfig")
                    {
                            codeStr +=
$@"
        private string[] blocks = null;

        private int blockIndex = 0;

        private int blockIndexStep = 0;

        public string Selector()
        {{
            string language = chs;
            if (blockIndex == -1)
            {{
                return language;
            }}
            else
            {{
                if (blocks == null)
                {{
                    if (language.Contains({qm}|{qm}))
                    {{
                        blocks = language.Split('|');
                        blockIndex = UnityEngine.Random.Range(0, blocks.Length);
                        blockIndexStep = UnityEngine.Random.Range(1, blocks.Length);
                    }}
                    else
                    {{
                        blockIndex = -1;
                        return language;
                    }}
                }}

                var txt = blocks[blockIndex];
                blockIndex += blockIndexStep;
                blockIndex %= blocks.Length;
                return txt;
            }}
        }}
";
                    }

                    codeStr +=
$@"
    }}
}}
";

                    var codeStrSavePath = "Assets/GameRes/GamePlayScript/Data/" + className + ".cs";
                    var lastCodeStr = string.Empty;
                    if (File.Exists(codeStrSavePath))
                    {
                        lastCodeStr = File.ReadAllText(codeStrSavePath);
                    }
                    if (lastCodeStr != codeStr)
                    {
                        File.WriteAllText(codeStrSavePath, codeStr);
                        AssetDatabase.ImportAsset(codeStrSavePath);
                    }
                    }

                    if (generateJson)
                    {
                    var qm = "\"";
                    var jsonStr =
$@"
{{
    {qm}keys{qm}:[";
                    for (int rowI = 3; rowI < table.Rows.Count; rowI++)
                    {
                        var row = table.Rows[rowI];
                        var firstCell = row.ItemArray[0].ToString();

                        jsonStr +=
$@"
        {qm}{firstCell}{qm},";
                    }

                    jsonStr = jsonStr.Substring(0, jsonStr.Length - 1);

                    jsonStr +=
$@"
    ],";

                    jsonStr +=
$@"
    {qm}values{qm}:[";

                    for (int rowI = 3; rowI < table.Rows.Count; rowI++)
                    {
                        jsonStr +=
$@"
        {{";

                        var row = table.Rows[rowI];
                        var cells = row.ItemArray;
                        Utils.Assert(propertyTypes.Count == cells.Length);
                        for (int cellI = 0; cellI < cells.Length; cellI++)
                        {
                            if (string.IsNullOrWhiteSpace(propertyTypes[cellI]) == false)
                            {
                                if (propertyTypes[cellI] == "string")
                                {
                                    jsonStr +=
$@"
            {qm}_{propertyNames[cellI]}{qm}:{qm}{cells[cellI]}{qm},";
                                }
                                else
                                {
                                    jsonStr +=
$@"
            {qm}_{propertyNames[cellI]}{qm}:{cells[cellI]},";
                                    }
                            }
                        }

                        jsonStr = jsonStr.Substring(0, jsonStr.Length - 1);

                        jsonStr +=
$@"
        }},";
                    }

                    jsonStr = jsonStr.Substring(0, jsonStr.Length - 1);

                    jsonStr +=
$@"
    ]";

                    jsonStr +=
$@"
}}
";

                    var jsonStrSavePath = folderPath + objectName + ".json";
                    var lastJsonStr = string.Empty;
                    if (File.Exists(jsonStrSavePath))
                    {
                        lastJsonStr = File.ReadAllText(jsonStrSavePath);
                    }
                    if (lastJsonStr != jsonStr)
                    {
                        File.WriteAllText(jsonStrSavePath, jsonStr);
                        AssetDatabase.ImportAsset(jsonStrSavePath);
                    }
                    }
                }
            }
        }

        private static string DefaultValueByType(string type)
        {
            return
                type == "string" ? "string.Empty" :
                type == "bool" ? "false" : "0";
        }
    }
}
#endif