using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    [CreateNodeMenu("Basic/Compare")]
    public class CompareNode : StoryNodeBase
    {
        public enum Operation
        {
            Equal = 1,
            NotEqual = 2, 
            Greater = 3,
            GreaterEqual = 4,
            Less = 5,
            LessEqual = 6
        }

        [Tooltip("Compare number left side")]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public float i_n_a = 0;

        [Tooltip("Compare number right side")]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public float i_n_b = 0;

        [Tooltip("Compare boolean left side")]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public bool i_b_a = false;

        [Tooltip("Compare boolean right side")]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public bool i_b_b = false;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public bool o_n = false;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public bool o_n_inv = false;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public bool o_b = false;

        [Output(typeConstraint = TypeConstraint.Strict)]
        public bool o_b_inv = false;

        public Operation operation = Operation.Equal;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "o_n")
            {
                return GetValue_o_n();
            }
            else if (port.fieldName == "o_n_inv")
            {
                return !GetValue_o_n();
            }
            else if (port.fieldName == "o_b")
            {
                return GetValue_o_b();
            }
            else if (port.fieldName == "o_b_inv")
            {
                return !GetValue_o_b();
            }
            else
            {
                return false;
            }
        }

        private bool GetValue_o_n()
        {
            var a = GetInputValue("i_n_a", i_n_a);
            var b = GetInputValue("i_n_b", i_n_b);
            if (operation == Operation.Equal)
            {
                return a == b;
            }
            else if (operation == Operation.NotEqual)
            {
                return a != b;
            }
            else if (operation == Operation.Greater)
            {
                return a > b;
            }
            else if (operation == Operation.GreaterEqual)
            {
                return a >= b;
            }
            else if (operation == Operation.Less)
            {
                return a < b;
            }
            else if (operation == Operation.LessEqual)
            {
                return a <= b;
            }
            else
            {
                return false;
            }
        }

        private bool GetValue_o_b()
        {
            var a = GetInputValue("i_b_a", i_b_a);
            var b = GetInputValue("i_b_b", i_b_b);
            if (operation == Operation.Equal)
            {
                return a == b;
            }
            else if (operation == Operation.NotEqual)
            {
                return a != b;
            }
            else
            {
                return false;
            }
        }
    }
}
