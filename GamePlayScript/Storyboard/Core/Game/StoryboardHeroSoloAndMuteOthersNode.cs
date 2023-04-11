using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using GameScript;

namespace StoryboardCore
{
    [CreateNodeMenu("Game/Storyboard/HeroSoloAndMuteOthers")]
    public class StoryboardHeroSoloAndMuteOthersNode : StoryNodeBase, ITriggerableNode
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public TriggerPort prev = null;

        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public bool i_heroSoloAndMuteOthers = false;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public bool o_heroSoloAndMuteOthers = false;

        public override object GetValue(NodePort port)
        {
            if (Application.isPlaying)
            {
                if (port.fieldName == "o_heroSoloAndMuteOthers")
                {
                    return DataCenter.GetInstance().bloackboard.heroSoloAndMuteOthers;
                }
                else
                {
                    Utils.Assert(false, "Unexpected port name");
                    return false;
                }
            }
            else
            {
                // Return a default value when game is not playing
                return false;
            }
        }

        private void SetValue()
        {
            if (Application.isPlaying)
            {
                DataCenter.GetInstance().bloackboard.heroSoloAndMuteOthers = GetInputValue("i_heroSoloAndMuteOthers", i_heroSoloAndMuteOthers);
            }
            else
            {
                // Do nothing when game is not playing
            }
        }

        public void TriggerOn(Action completeCallback)
        {
            Utils.Assert(completeCallback != null);
            SetValue();
            completeCallback();
        }
    }
}
