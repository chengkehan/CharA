using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using GameScript;
using GameScript.Cutscene;

namespace StoryboardCore
{
    [CreateNodeMenu("Game/BoundsTrigger")]
    public class BoundsTriggerNode : StoryNodeBase, ITriggerableNode
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public TriggerPort prev = null;

        [Tooltip("GUID of BoundsTrigger")]
        [Input(typeConstraint = TypeConstraint.Strict)]
        [GuidMono]
        public string guidBT = string.Empty;

        [Tooltip("Enabled of BoundsTrigger")]
        [Input(typeConstraint = TypeConstraint.Strict)]
        public bool enabledBT = true;

        public void TriggerOn(Action completeCallback)
        {
            if (Application.isPlaying)
            {
                var allBoundsTriggers = GameObject.FindObjectsOfType<BoundsTrigger>(true);
                foreach (var boundsTrigger in allBoundsTriggers)
                {
                    if (boundsTrigger.guid == guidBT)
                    {
                        var enabled = GetInputValue("enabledBT", enabledBT);
                        boundsTrigger.enabled = enabled;
                        boundsTrigger.gameObject.SetActive(enabled);
                        break;
                    }
                }
            }
            else
            {
                // Do nothing when game is not playing
            }

            completeCallback();
        }
    }
}
