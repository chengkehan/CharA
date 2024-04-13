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

        // Brain can do only one thing at a time.
        // When brain is executing a behavior, this flag should be set,
        // and reset this flag once behavior is complete.
        private bool _isBusy = false;
        public bool isBusy
        {
            set
            {
                _isBusy = value;
            }
            get
            {
                return _isBusy;
            }
        }

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
