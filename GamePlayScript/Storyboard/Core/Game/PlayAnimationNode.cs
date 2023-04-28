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

        [Tooltip("Which up body animation will be played.\nUsing this as finishing signal.")]
        public UpBodySM.Transition upBodyAnimation = UpBodySM.Transition.None;

        [Tooltip("Which up body2 animation will be played.\nUsing this as finishing signal.")]
        public UpBody2SM.Transition upBody2Animation = UpBody2SM.Transition.None;

        [Tooltip("Which up body3 animation will be played.\nUsing this as finishing signal.")]
        public UpBody3SM.Transition upBody3Animation = UpBody3SM.Transition.None;

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
            this.completeCallback = completeCallback;

            string roleId = DataCenter.query.ProcessRoleId(this.roleId);

            Actor actor = ActorsManager.GetInstance().GetActor(roleId);
            if (actor == null)
            {
                Utils.LogObservably("Storyboard, PlayAnimationNode, Failed, actor not found. " + roleId);
                ExecuteCompleteCallback();
            }
            else
            {
                if (animation == SoloSM.Transition.Dynamic)
                {
                    var loadedAnimation = AssetsManager.GetInstance().LoadAnimation(dynamic);
                    actor.roleAnimation.GetMotionAnimator().SetDynamic(loadedAnimation, MotionAnimator.DynamicAnimation.DynamicSolo);
                    actor.roleAnimation.GetMotionAnimator().SetSoloState(animation);
                }
                else
                {
                    if (animation != SoloSM.Transition.Undefined)
                    {
                        actor.roleAnimation.GetMotionAnimator().SetSoloState(animation);
                    }
                }

                if (upBodyAnimation == UpBodySM.Transition.Dynamic)
                {
                    var loadedAnimation = AssetsManager.GetInstance().LoadAnimation(dynamic);
                    actor.roleAnimation.GetMotionAnimator().SetDynamic(loadedAnimation, MotionAnimator.DynamicAnimation.DynamicUpBody);
                    actor.roleAnimation.GetMotionAnimator().SetUpBodyAnimation(upBodyAnimation);
                }
                else
                {
                    if (upBodyAnimation != UpBodySM.Transition.None)
                    {
                        actor.roleAnimation.GetMotionAnimator().SetUpBodyAnimation(upBodyAnimation);
                    }
                }

                if (upBody2Animation == UpBody2SM.Transition.Dynamic)
                {
                    var loadedAnimation = AssetsManager.GetInstance().LoadAnimation(dynamic);
                    actor.roleAnimation.GetMotionAnimator().SetDynamic(loadedAnimation, MotionAnimator.DynamicAnimation.DynamicUpBody2);
                    actor.roleAnimation.GetMotionAnimator().SetUpBody2Animation(upBody2Animation);
                }
                else
                {
                    if (upBody2Animation != UpBody2SM.Transition.None)
                    {
                        actor.roleAnimation.GetMotionAnimator().SetUpBody2Animation(upBody2Animation);
                    }
                }

                if (upBody3Animation == UpBody3SM.Transition.Dynamic)
                {
                    var loadedAnimation = AssetsManager.GetInstance().LoadAnimation(dynamic);
                    actor.roleAnimation.GetMotionAnimator().SetDynamic(loadedAnimation, MotionAnimator.DynamicAnimation.DynamicUpBody3);
                    actor.roleAnimation.GetMotionAnimator().SetUpBody3Animation(upBody3Animation);
                }
                else
                {
                    if (upBody3Animation != UpBody3SM.Transition.None)
                    {
                        actor.roleAnimation.GetMotionAnimator().SetUpBody3Animation(upBody3Animation);
                    }
                }
            }

            if (waitingForComplete)
            {
                if (timeout == 0)
                {
                    if (finishingSignal != SoloSM.Transition.Undefined ||
                        upBodyAnimation != UpBodySM.Transition.None ||
                        upBody2Animation != UpBody2SM.Transition.None ||
                        upBody3Animation != UpBody3SM.Transition.None)
                    {
                        EventSystem.GetInstance().AddListener(EventID.SoloComplete, SoloCompleteHandler);
                        EventSystem.GetInstance().AddListener(EventID.UpBodyAnimationComplete, UpBodyAnimationCompleteHandler);
                    }
                }
                else
                {
                    Coroutines.GetInstance().Execute(TimeoutCoroutine());
                }
            }
            else
            {
                ExecuteCompleteCallback();
            }
        }

        private void UpBodyAnimationCompleteHandler(NotificationData _data)
        {
            var data = _data as UpBodyAnimationCompleteND;
            if (data != null)
            {
                if (DataCenter.query.ProcessRoleId(roleId) == DataCenter.query.ProcessRoleId(data.roleId))
                {
                    if (data.upBodyAnimation == upBodyAnimation || data.upBodyAnimation2 == upBody2Animation)
                    {
                        Complete();
                    }
                }
            }
        }

        private void SoloCompleteHandler(NotificationData _data)
        {
            var data = _data as SoloCompleteND;
            if (data != null)
            {
                if (DataCenter.query.ProcessRoleId(roleId) == DataCenter.query.ProcessRoleId(data.roleId))
                {
                    if (data.transition == finishingSignal)
                    {
                        Complete();
                    }
                }
            }
        }

        private void Complete()
        {
            EventSystem.GetInstance().RemoveListener(EventID.SoloComplete, SoloCompleteHandler);
            EventSystem.GetInstance().RemoveListener(EventID.UpBodyAnimationComplete, UpBodyAnimationCompleteHandler);

            var roleId = DataCenter.query.ProcessRoleId(this.roleId);
            Actor actor = ActorsManager.GetInstance().GetActor(roleId);
            CleanupAnimation(actor);
            ExecuteCompleteCallback();
        }

        private IEnumerator TimeoutCoroutine()
        {
            yield return new WaitForSeconds(timeout);
            Complete();
        }

        private void CleanupAnimation(Actor actor)
        {
            actor.roleAnimation.GetMotionAnimator().SetUpBodyAnimation(UpBodySM.Transition.None);
            actor.roleAnimation.GetMotionAnimator().SetUpBody2Animation(UpBody2SM.Transition.None);
            actor.roleAnimation.GetMotionAnimator().SetUpBody3Animation(UpBody3SM.Transition.None);

            actor.roleAnimation.GetMotionAnimator().ClearDynamic(MotionAnimator.DynamicAnimation.DynamicSolo);
            actor.roleAnimation.GetMotionAnimator().ClearDynamic(MotionAnimator.DynamicAnimation.DynamicUpBody);
            actor.roleAnimation.GetMotionAnimator().ClearDynamic(MotionAnimator.DynamicAnimation.DynamicUpBody2);
            actor.roleAnimation.GetMotionAnimator().ClearDynamic(MotionAnimator.DynamicAnimation.DynamicUpBody3);
        }

        private void ExecuteCompleteCallback()
        {
            if (completeCallback != null)
            {
                completeCallback();
                completeCallback = null;
            }
        }
    }
}
