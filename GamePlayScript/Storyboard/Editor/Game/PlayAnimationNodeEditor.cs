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

            var animationMutex =
                (node.animation != SoloSM.Transition.Undefined && node.upBodyAnimation == MotionAnimator.UpBodyAnimation.None && node.upBody2Animation == MotionAnimator.UpBodyAnimationLayer2.None) ||
                (node.upBodyAnimation != MotionAnimator.UpBodyAnimation.None && node.animation == SoloSM.Transition.Undefined && node.upBody2Animation == MotionAnimator.UpBodyAnimationLayer2.None) ||
                (node.upBody2Animation != MotionAnimator.UpBodyAnimationLayer2.None && node.animation == SoloSM.Transition.Undefined && node.upBodyAnimation == MotionAnimator.UpBodyAnimation.None);

            var finishingSignal =
                node.upBodyAnimation != MotionAnimator.UpBodyAnimation.None || node.upBody2Animation != MotionAnimator.UpBodyAnimationLayer2.None ?
                true :
                !(node.timeout == 0 && node.finishingSignal == SoloSM.Transition.Undefined && node.waitingForComplete);

            Validate(false,
                !string.IsNullOrWhiteSpace(node.roleId), "role id is required",
                animationMutex, "Missing Animation Setting.\nAnimation | UpBodyAnimation | UpBody2Animation, choose one of three.",
                node.animation != SoloSM.Transition.Dynamic ? true : !string.IsNullOrWhiteSpace(node.dynamic), "dynamic is required",
                finishingSignal, "missing exiting condition"
            );
        }
    }
}
#endif
