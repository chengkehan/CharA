#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using StoryboardCore;

namespace StoryboardEditor
{
    [CustomNodeEditor(typeof(ThreadValueNode))]
    public class ThreadValueNodeEditor : GameNodeEditorBase<ThreadValueNode>
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            DrawRuntimeOnlyTips();
        }

        public override void OnValidate()
        {
            base.OnValidate();

            Validate(false,
                !string.IsNullOrWhiteSpace(node.valueName), "value name is required"
            );
        }
    }
}
#endif
