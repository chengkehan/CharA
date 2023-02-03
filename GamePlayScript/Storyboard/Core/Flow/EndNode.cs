using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    [CreateNodeMenu("Flow/End")]
    public class EndNode : StoryNodeBase
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public FlowPort prev = null;
    }
}
