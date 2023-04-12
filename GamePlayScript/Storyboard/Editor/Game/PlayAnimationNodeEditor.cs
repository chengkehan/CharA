#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using StoryboardCore;
using GameScript;

namespace StoryboardEditor
{
    [CustomNodeEditor(typeof(PlayAnimationNode))]
    public class PlayAnimationNodeEditor : GameNodeEditorBase<PlayAnimationNode>
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            DrawRuntimeOnlyTips();
            DrawTipsLabel("Solo animations only", null);
            DrawRoleHeadIcon(node.roleId);
        }

        public override void OnValidate()
        {
            base.OnValidate();

            Validate(false,
                !string.IsNullOrWhiteSpace(node.roleId), "role id is required",
                node.animation != SoloSM.Transition.Undefined, "animation is required",
                node.animation != SoloSM.Transition.Dynamic ? true : !string.IsNullOrWhiteSpace(node.dynamic), "dynamic is required",
                !(node.timeout == 0 && node.finishingSignal == SoloSM.Transition.Undefined && node.waitingForComplete), "missing exiting condition"
            );
        }
    }
}
#endif
