using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Trees;
using CleverCrow.Fluid.BTs.Tasks;

namespace GameScript
{
    public abstract class AI : MonoBehaviour
    {
        public abstract BehaviorTree Get();

        private NpcBrain _npcBrain = null;
        protected NpcBrain npcBrain
        {
            get
            {
                if (_npcBrain == null)
                {
                    if (gameObject != null)
                    {
                        _npcBrain = gameObject.GetComponent<NpcBrain>();
                    }
                }
                return _npcBrain;
            }
        }
    }
}
