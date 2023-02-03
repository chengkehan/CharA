using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace StoryboardCore
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConditionDisableAttribute : PropertyAttribute
    {
        public string conditionalSourceField = string.Empty;

        public bool comparer = false;

        public ConditionDisableAttribute(string conditionalSourceField, bool comparer)
        {
            this.conditionalSourceField = conditionalSourceField;
            this.comparer = comparer;
        }
    }
}
