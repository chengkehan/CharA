#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using GameScript;

namespace GameScriptEditor
{
    public class GuidGenerator
    {
        [MenuItem("GameTools/GUID Generator")]
        private static void Execute()
        {
            string guid = Guid.NewGuid().ToString();
            Utils.Log(guid);
            UnityEngine.GUIUtility.systemCopyBuffer = guid;

        }
    }
}
#endif
