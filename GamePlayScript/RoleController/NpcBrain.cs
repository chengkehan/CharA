using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Trees;
using CleverCrow.Fluid.BTs.Tasks;

namespace GameScript
{
    public class NpcBrain : RoleAnimation
    {
        [SerializeField]
        private BehaviorTree behaviorTree = null;

        protected override void Awake()
        {
            base.Awake();

            AI ai = gameObject.GetComponent<AI>();
            if (ai != null && ai.Get() != null)
            {
                behaviorTree = new BehaviorTreeBuilder(gameObject)
                    .Splice(ai.Get())
                .Build();
            }
        }

        protected override void Update()
        {
            base.Update();

            if (behaviorTree != null)
            {
                behaviorTree.Tick();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            behaviorTree = null;
        }
    }
}
