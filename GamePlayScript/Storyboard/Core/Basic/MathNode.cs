using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    [CreateNodeMenu("Basic/Math")]
    public class MathNode : StoryNodeBase
    {
        public enum Operation
        {
            Add = 1,
            Subtract = 2,
            Multiply = 3,
            Divide = 4
        }

        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public float a = 0;

        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public float b = 0;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public float result = 0;

        public Operation operation = Operation.Add;

        public override object GetValue(NodePort port)
        {
            float a = GetInputValue<float>("a", this.a);
            float b = GetInputValue<float>("b", this.b);

            result = 0;
            switch (operation)
            {
                case Operation.Add: default:
                    {
                        result = a + b;
                        break;
                    }
                case Operation.Subtract:
                    {
                        result = a - b;
                        break;
                    }
                case Operation.Multiply:
                    {
                        result = a * b;
                         break;
                    }
                case Operation.Divide:
                    {
                        if (Mathf.Approximately(b, 0))
                        {
                            result = 0;
                        }
                        else
                        {
                            result = a / b;
                        }
                        break;
                    }
            }
            return result;
        }
    }
}
