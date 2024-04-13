using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Trees;
using CleverCrow.Fluid.BTs.Tasks;

namespace GameScript
{
    public static class BehaviorTreeBuilderExtensions
    {
        //----------- Animation ------------------------------------------------

        public static BehaviorTreeBuilder ActionSittingGroundDown(this BehaviorTreeBuilder builder)
        {
            return builder.AddNode(new ActionSittingGroundDown
            {
                Name = "ActionSittingGroundDown"
            });
        }

        //----------- Brain ------------------------------------------------

        public static BehaviorTreeBuilder ActionSetBrainBusy(this BehaviorTreeBuilder builder)
        {
            return builder.AddNode(new ActionSetBrainBusy
            {
                Name = "ActionSetBrainBusy"
            });
        }
    }
}
