using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameScript
{
    public class AssertException : Exception
    {
        public AssertException() : base()
        {
            // Do nothing
        }

        public AssertException(string message) : base(message)
        {
            // Do nothing
        }
    }
}
