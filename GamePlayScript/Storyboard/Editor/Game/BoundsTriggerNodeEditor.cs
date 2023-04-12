#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using StoryboardCore;
using GameScript.Cutscene;

namespace StoryboardEditor
{
    [CustomNodeEditor(typeof(BoundsTriggerNode))]
    public class BoundsTriggerNodeEditor : GameNodeEditorBase<BoundsTriggerNode>
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (GUILayout.Button("Goto BoundsTrigger"))
            {
                var allBoundsTriggers = GameObject.FindObjectsOfType<BoundsTrigger>(true);
                foreach (var boundsTrigger in allBoundsTriggers)
                {
                    if (boundsTrigger.guid == node.guidBT)
                    {
                        Selection.activeGameObject = boundsTrigger.gameObject;
                        break;
                    }
                }
            }
        }
    }
}
#endif
