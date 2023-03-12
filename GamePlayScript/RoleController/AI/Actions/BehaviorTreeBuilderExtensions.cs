using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Trees;
using CleverCrow.Fluid.BTs.Tasks;

namespace GameScript
{
    public static class BehaviorTreeBuilderExtensions
    {
        public static BehaviorTreeBuilder ActionSittingGroundDown(this BehaviorTreeBuilder builder)
        {
            return builder.AddNode(new ActionSittingGroundDown
            {
                Name = "ActionSittingGroundDown"
            });
        }
    }
}
