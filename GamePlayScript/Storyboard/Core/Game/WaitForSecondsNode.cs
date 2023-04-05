using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using GameScript;

namespace StoryboardCore
{
    [CreateNodeMenu("Game/Wait For Seconds")]
    public class WaitForSecondsNode : StoryNodeBase, ITriggerableNode
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public TriggerPort prev = null;

        [Tooltip("How many seconds to paused")]
        [Min(0)]
        public float seconds = 1;

        public void TriggerOn(Action completeCallback)
        {
            Utils.Assert(completeCallback != null);

            Coroutines.GetInstance().Execute(WaitingCoroutine(completeCallback));
        }

        private IEnumerator WaitingCoroutine(Action completeCallback)
        {
            yield return new WaitForSeconds(seconds);

            completeCallback();
        }
    }
}
