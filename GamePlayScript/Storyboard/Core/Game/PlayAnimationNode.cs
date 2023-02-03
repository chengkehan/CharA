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

        [Tooltip("Name of animation")]
        public string animationName = string.Empty;

        [Tooltip("Sleep thread until animation is complete.")]
        public bool waitingForComplete = true;

        [Tooltip("When animation is never complete, for example it's type is loop, we stop it forcedly after this time.")]
        public float timeout = 10;

        public void TriggerOn(Action completeCallback)
        {
            Utils.Assert(completeCallback != null);

            // TODO play animation
            Debug.Log("PlayAnimationNode TriggerOn");

            completeCallback();
        }
    }
}
