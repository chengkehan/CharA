using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    [CreateNodeMenu("Flow/Talk")]
    public class TalkNode : StoryNodeBase
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public FlowPort prev = null;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public FlowPort next = null;

        [Tooltip("Who's talking?\nLeave this filed empty to mark words is thinking in hero's mind.")]
        public string roleId = string.Empty;

        [Tooltip("Id of words.\nLanguages.xsls")]
        public string wordsId = string.Empty;
    }
}
