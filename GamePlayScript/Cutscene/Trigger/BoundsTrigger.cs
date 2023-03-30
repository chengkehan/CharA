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

        [SerializeField]
        private InterfaceReference<IBoundsTriggerTarget, MonoBehaviour> target = null;

        [SerializeField]
        private BoundsComponent bounds = new BoundsComponent();

        private bool isInBounds = false;

        private string _targetActorGUID = null;
        public string targetActorGUID
        {
            set
            {
                if (_targetActorGUID != value)
                {
                    isInBounds = false;
                    _targetActorGUID = value;
                }
            }
            get
            {
                return _targetActorGUID;
            }
        }

        public void Update()
        {
            Actor actor = null;

            if (ActorsManager.GetInstance() != null)
            {
                // Seem it as hero
                if (targetActorGUID == null)
                {
                    actor = ActorsManager.GetInstance().GetHeroActor();
                }
                // Seem it as npc
                else
                {
                    actor = ActorsManager.GetInstance().GetActorByGUID(targetActorGUID);
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
                if (target != null && target.Value != null)
                {
                    target.Value.Triggger();
                }
            }
        }

        private void OnExit()
        {
            if (triggerType == TriggerType.OnExit)
            {
                if (target != null && target.Value != null)
                {
                    target.Value.Triggger();
                }
            }
        }
    }
}
