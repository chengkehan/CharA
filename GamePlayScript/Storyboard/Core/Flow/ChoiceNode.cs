using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    [CreateNodeMenu("Flow/Choice")]
    public class ChoiceNode : StoryNodeBase
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public FlowPort prev = null;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public FlowPort next = null;

        public string wordsId = string.Empty;

        [Tooltip("Sorting parallel choices by this")]
        public int order = 0;
    }
}
