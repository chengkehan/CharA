using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class BoundsTrigger : SerializableMonoBehaviour<BoundsTriggerPD>
    {
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

            }
        }
    }
}
