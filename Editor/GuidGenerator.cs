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
            Utils.Log(Guid.NewGuid().ToString());
        }
    }
}
#endif
