using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    [CreateNodeMenu("Flow/Start")]
    public class StartNode : StoryNodeBase
    {
        [Output(typeConstraint = TypeConstraint.Strict)]
        public FlowPort next = null;
    }
}
