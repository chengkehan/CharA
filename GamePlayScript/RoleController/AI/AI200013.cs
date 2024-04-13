using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Trees;
using CleverCrow.Fluid.BTs.Tasks;

namespace GameScript
{
    public class AI200013 : AI
    {
        private BehaviorTree behaviorTree = null;

        public override BehaviorTree Get()
        {
            if (behaviorTree == null)
            {
                behaviorTree = new BehaviorTreeBuilder(gameObject)
                    .Sequence()
                        .Condition("Not busy", () => !npcBrain.isBusy)
                        .ActionSetBrainBusy()
                        .ActionSittingGroundDown()
                .Build();
            }
            return behaviorTree;
        }
    }
}
