using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    [CreateNodeMenu("Basic/Boolean")]
    public class BooleanNode : StoryNodeBase
    {
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public bool i = true;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public bool o = true;

        public bool inversed = false;

        public override object GetValue(NodePort port)
        {
            var output = GetInputValue("i", i);
            if (inversed)
            {
                output = !output;
            }
            return output;
        }
    }
}
