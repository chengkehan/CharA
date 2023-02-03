using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    [CreateNodeMenu("Basic/String")]
    public class StringNode : StoryNodeBase
    {
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public string i = string.Empty;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public string o = string.Empty;

        public override object GetValue(NodePort port)
        {
            return GetInputValue("i", i);
        }
    }
}
