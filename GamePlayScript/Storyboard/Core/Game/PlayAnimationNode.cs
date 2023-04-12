using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using GameScript;

namespace StoryboardCore
{
    [CreateNodeMenu("Game/Play Animation")]
    public class PlayAnimationNode : StoryNodeBase, ITriggerableNode
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public TriggerPort prev = null;

        [Tooltip("Who's playing animation?")]
        public string roleId = string.Empty;

        [Tooltip("Which animation will be played.")]
        public SoloSM.Transition animation = SoloSM.Transition.Undefined;

        [Tooltip("Name of AnimatorOverrideController when animation is dynamic.")]
        public string dynamic = string.Empty;

        [Tooltip("Sleep thread until animation is complete.")]
        public bool waitingForComplete = true;

        [Tooltip("When animation is never complete, for example it's type is loop, we stop it forcedly after this time.\n0 means never timeout.")]
        [Min(0)]
        public float timeout = 0;

        [Tooltip("Exit/Complete animation when receive this signal.")]
        public SoloSM.Transition finishingSignal = SoloSM.Transition.Undefined;

        private Action completeCallback = null;

        public void TriggerOn(Action completeCallback)
        {
            Utils.Assert(completeCallback != null);

            string roleId = DataCenter.query.ProcessRoleId(this.roleId);

            Actor actor = ActorsManager.GetInstance().GetActor(roleId);
            if (actor == null)
            {
                Utils.LogObservably("Storyboard, PlayAnimationNode, Failed, actor not found. " + roleId);
                completeCallback();
            }
            else
            {
                actor.roleAnimation.GetMotionAnimator().SetSoloState(animation);
            }

            if (waitingForComplete)
            {
                if (timeout == 0)
                {
                    if (finishingSignal != SoloSM.Transition.Undefined)
                    {
                        this.completeCallback = completeCallback;
                        EventSystem.GetInstance().AddListener(EventID.LoopTypeSoloComplete, LoopTypeSoloCompleteHandler);
                    }
                }
                else
                {
                    Coroutines.GetInstance().StartCoroutine(TimeoutCoroutine(completeCallback));
                }
            }
            else
            {
                completeCallback();
            }
        }

        private void LoopTypeSoloCompleteHandler(NotificationData _data)
        {
            var data = _data as LoopTypeSoloCompleteND;
            if (data != null)
            {
                if (DataCenter.query.ProcessRoleId(roleId) == DataCenter.query.ProcessRoleId(data.roleId))
                {
                    if (data.transition == finishingSignal)
                    {
                        EventSystem.GetInstance().RemoveListener(EventID.LoopTypeSoloComplete, LoopTypeSoloCompleteHandler);
                        completeCallback();
                        completeCallback = null;
                    }
                }
            }
        }

        private IEnumerator TimeoutCoroutine(Action completeCallback)
        {
            yield return new WaitForSeconds(timeout);

            completeCallback();
        }
    }
}
