using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    [CreateNodeMenu("Flow/Trigger")]
    public class TriggerNode : StoryNodeBase
    {
        public const string TRIGGER_PORT_NAME = "trigger";

        [Input(typeConstraint = TypeConstraint.Strict)]
        public FlowPort prev = null;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public FlowPort next = null;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public TriggerPort trigger = null;
    }
}
