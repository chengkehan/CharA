using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using AYellowpaper;

namespace GameScript.Cutscene
{
    public class BoundsTrigger : SerializableMonoBehaviour<BoundsTriggerPD>
    {
        public enum TriggerType
        {
            OnEnter,
            OnExit
        }

        public TriggerType triggerType = TriggerType.OnEnter;

        [Tooltip("How many times can be triggered. \nZero means infinite times.")]
        [Min(0)]
        public int triggeredMaxTimes = 0;

        [SerializeField]
        private InterfaceReference<IBoundsTriggerTarget, MonoBehaviour> target = null;

        [Tooltip("Who will trigger it.\nLeave it empty for hero.")]
        [SerializeField]
        private string targetRoleId = null;

        [SerializeField]
        private BoundsComponent bounds = new BoundsComponent();

        private bool isInBounds = false;

        public void Update()
        {
            Actor actor = null;

            if (ActorsManager.GetInstance() != null)
            {
                // Seem it as hero
                if (string.IsNullOrEmpty(targetRoleId))
                {
                    actor = ActorsManager.GetInstance().GetHeroActor();
                }
                // Seem it as npc
                else
                {
                    actor = ActorsManager.GetInstance().GetActor(targetRoleId);
                }
            }

            if (actor != null)
            {
                var isActorInBounds = bounds.InBounds(actor.roleAnimation.GetMotionAnimator().GetPosition());
                if (isActorInBounds)
                {
                    if (isInBounds == false)
                    {
                        isInBounds = true;
                        OnEnter();
                    }
                }
                else
                {
                    if (isInBounds)
                    {
                        isInBounds = false;
                        OnExit();
                    }
                }
            }
        }

        private void OnEnter()
        {
            if (triggerType == TriggerType.OnEnter)
            {
                if (triggeredMaxTimes == 0 || pd.triggeredTimes < triggeredMaxTimes)
                {
                    ++pd.triggeredTimes;
                    if (target != null && target.Value != null)
                    {
                        target.Value.Triggger();
                    }
                }
            }
        }

        private void OnExit()
        {
            if (triggerType == TriggerType.OnExit)
            {
                if (triggeredMaxTimes == 0 || pd.triggeredTimes < triggeredMaxTimes)
                {
                    ++pd.triggeredTimes;
                    if (target != null && target.Value != null)
                    {
                        target.Value.Triggger();
                    }
                }
            }
        }
    }
}
