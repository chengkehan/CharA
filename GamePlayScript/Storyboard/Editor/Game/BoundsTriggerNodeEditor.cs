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
using GameScriptEditor;

namespace StoryboardEditor
{
    [CustomNodeEditor(typeof(BoundsTriggerNode))]
    public class BoundsTriggerNodeEditor : GameNodeEditorBase<BoundsTriggerNode>
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
        }

        public override void OnValidate()
        {
            base.OnValidate();

            if (string.IsNullOrEmpty(node.guidBT) == false)
            {
                var boundsTrigger = MasterEditor.Find<BoundsTrigger>(node.guidBT);

                Validate(false,
                    boundsTrigger != null, "Invalid guid"
                );
            }
        }
    }
}
#endif
