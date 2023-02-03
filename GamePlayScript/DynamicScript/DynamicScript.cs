using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MoonSharp.Interpreter;

namespace GameScript
{
    /*
     * IsHeroObserving() : global
     */

    public class DynamicScript
    {
        private static DynamicScript s_instance = null;

        public static DynamicScript GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new DynamicScript();
            }
            return s_instance;
        }

        private string _npcId = null;
        public string npcId
        {
            get
            {
                return _npcId;
            }
            set
            {
                _npcId = value;
            }
        }

        // Execute a script and return a value.
        public bool Execute(string script, bool returnIsExpectant)
        {
            Script.DefaultOptions.DebugPrint = Utils.Log;

            script = script == null ? string.Empty : script.Trim();
            if (string.IsNullOrWhiteSpace(script))
            {
                npcId = null;
                return true;
            }
            else
            {
                if (returnIsExpectant)
                {
                    string[] lines = script.Split('\n');
                    for (int i = lines.Length - 1; i >= 0; --i)
                    {
                        string line = lines[i];
                        if (string.IsNullOrWhiteSpace(line) == false && line.Contains("return") == false)
                        {
                            line = "return " + line.Trim();
                            lines[i] = line;
                        }
                    }
                    script = string.Join("\n", lines);
                }

                //Utils.Log("Dynamic script : " + script);

                // Execute script
                Script luaScript = new Script();

                // Register Global Functions
                luaScript.Globals["IsHeroObserving"] = (Func<bool>)DataCenter.GetInstance().IsHeroObserving;

                if (string.IsNullOrEmpty(npcId) == false)
                {
                    luaScript.Globals["npcId"] = npcId;
                    npcId = null;
                }
                DynValue o = luaScript.DoString(script);

                return o.Boolean;
            }

            return false;
        }

        public void WriteToScript<TValue>(Script script, DataType type, string key, TValue value)
        {
            if (script != null)
            {
                script.Globals[key] = value;
            }
        }

        public void EnumerateScript(Script script, Action<DataType, string, DynValue> eachOneCB)
        {
            if (script != null)
            {
                foreach (var key in script.Globals.Keys)
                {
                    if (key.String != "_VERSION")
                    {
                        var value = script.Globals.Get(key);
                        var valueType = value.Type;
                        if (valueType == DataType.String || valueType == DataType.Boolean || valueType == DataType.Number)
                        {
                            eachOneCB?.Invoke(valueType, key.String, value);
                        }
                    }
                }
            }
        }
    }
}