#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameScriptEditor
{
    public class RandomRigidbodysParams
    {
        [MenuItem("GameTools/Random Rigidbodys Params")]
        private static void Execute()
        {
            var selectedGos = Selection.gameObjects;
            foreach (var selectedGo in selectedGos)
            {
                var rigidbody = selectedGo.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.mass = Random.Range(1.5f, 2.5f);
                    rigidbody.drag = Random.Range(3.5f, 4.8f);
                    EditorUtility.SetDirty(selectedGo);
                }
            }
        }
    }
}
#endif
