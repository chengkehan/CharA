using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    [CreateNodeMenu("Basic/Number")]
    public class NumberNode : StoryNodeBase
    {
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public float i = 0;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public float o = 0;

        public override object GetValue(NodePort port)
        {
            return GetInputValue("i", i);
        }
    }
}
