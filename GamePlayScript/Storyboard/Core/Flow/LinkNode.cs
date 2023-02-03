using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    [CreateNodeMenu("Flow/Link")]
    public class LinkNode : StoryNodeBase
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public FlowPort prev = null;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public FlowPort next = null;

        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public bool connected = true;

        public bool Connected()
        {
            return GetInputValue("connected", connected);
        }
    }
}
