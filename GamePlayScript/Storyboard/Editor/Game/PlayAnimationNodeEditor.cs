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
    [CustomNodeEditor(typeof(PlayAnimationNode))]
    public class PlayAnimationNodeEditor : GameNodeEditorBase<PlayAnimationNode>
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            DrawRuntimeOnlyTips();
            DrawRoleHeadIcon(node.roleId);
        }

        public override void OnValidate()
        {
            base.OnValidate();

            Validate(false,
                !string.IsNullOrWhiteSpace(node.roleId), "role id is required",
                !string.IsNullOrWhiteSpace(node.animationName), "animation name is required"
            );
        }
    }
}
#endif
