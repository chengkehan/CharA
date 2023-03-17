using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameScript
{
    public class Utils
    {
        public static void SetLayerRecursively(GameObject go, int layer)
        {
            if (go != null)
            {
                go.layer = layer;
                if (go.transform != null)
                {
                    for (int childI = 0; childI < go.transform.childCount; childI++)
                    {
                        SetLayerRecursively(go.transform.GetChild(childI).gameObject, layer);
                    }
                }
            }
        }

        public static T StringToEnum<T>(string str)
            where T : System.Enum
        {
            return (T)System.Enum.Parse(typeof(T), str);
        }

        public static int EnumToValue(System.Enum e)
        {
            return System.Convert.ToInt32(e);
        }

        public static string Serialize<T>(T obj)
        {
            return JsonUtility.ToJson(obj, true);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        public static T DeserializeOverwrite<T>(string json, T obj)
        {
            JsonUtility.FromJsonOverwrite(json, obj);
            return obj;
        }

        public static GameObject InstantiateUIPrefab(Object uiPrefab, Transform parent)
        {
            if (uiPrefab == null || parent == null)
            {
                return null;
            }
            var go = Object.Instantiate(uiPrefab) as GameObject;
            if (go == null)
            {
                return null;
            }
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            return go;
        }

        public static void Destroy(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            gameObject.transform.SetParent(null);
            GameObject.Destroy(gameObject);
        }

        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
            RaiseException(condition);
        }

        public static void Assert(bool condition, string message)
        {
            Debug.Assert(condition, message);
            RaiseException(condition, message);
        }

        public static void LogObservably(string message, bool condition = true, bool editorOnly = false, string color = "yellow")
        {
            if (editorOnly)
            {
#if UNITY_EDITOR
                if (condition)
                {
                    Log("<color=" + color + ">" + message + "</color>");
                }
#endif
            }
            else
            {
                if (condition)
                {
                    Log("<color=" + color + ">" + message + "</color>");
                }
            }
        }

        public static void Log(object message)
        {
            Debug.Log(message);
        }

        public static void Log(object message, Object context)
        {
            Debug.Log(message, context);
        }

        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        public static void LogWarning(object message, Object context)
        {
            Debug.LogWarning(message, context);
        }

        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        public static void LogError(object message, Object context)
        {
            Debug.LogError(message, context);
        }

        private static void RaiseException(bool condition)
        {
            if (condition == false)
            {
                throw new AssertException();
            }
        }

        private static void RaiseException(bool condition, string message)
        {
            if (condition == false)
            {
                throw new AssertException(message);
            }
        }

#if UNITY_EDITOR
        [UnityEditor.Callbacks.OnOpenAssetAttribute(-1)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            if (line != -1)
            {
                string filePath = "Assets/GameRes/GamePlayScript/Utils/Utils.cs";
                Object fileObject = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
                int fileInstanceId = fileObject.GetInstanceID();
                if (fileInstanceId == instanceID)
                {
                    string stackTrace = GetStackTrace();
                    string[] lines = stackTrace.Split('\n');
                    for (int lineI = 0; lineI < lines.Length; lineI++)
                    {
                        if (lines[lineI].Contains(filePath))
                        {
                            if (stackTrace.Contains("AssertException"))
                            {
                                lineI += 2;
                            }
                            else
                            {
                                lineI += 1;
                            }
                            if (lineI < lines.Length)
                            {
                                string[] blocks = lines[lineI].Split(new string[] { "Assets/GameRes/" }, System.StringSplitOptions.None);
                                string[] blocks2 = blocks[1].Split(new string[] { ":" }, System.StringSplitOptions.None);
                                string targetFilePath = "Assets/GameRes/" + blocks2[0];
                                string targetLineNumber = blocks2[1].Substring(0, blocks2[1].Length - 1);
                                Object targetFileObject = AssetDatabase.LoadAssetAtPath(targetFilePath, typeof(Object));
                                EditorGUIUtility.PingObject(targetFileObject);
                                AssetDatabase.OpenAsset(targetFileObject, int.Parse(targetLineNumber));
                            }
                            break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private static string GetStackTrace()
        {
            var consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            var fieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            var consoleWindowInstance = fieldInfo.GetValue(null);

            if (null != consoleWindowInstance)
            {
                if ((object)EditorWindow.focusedWindow == consoleWindowInstance)
                {
                    fieldInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                    string activeText = fieldInfo.GetValue(consoleWindowInstance).ToString();
                    return activeText;
                }
            }
            return string.Empty;
        }
#endif
    }
}

