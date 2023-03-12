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
    }
}
