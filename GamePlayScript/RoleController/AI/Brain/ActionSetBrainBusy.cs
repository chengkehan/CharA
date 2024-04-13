using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks;

namespace GameScript
{
    public class ActionSetBrainBusy : ActionBase
    {
        protected override void OnStart()
        {
            base.OnStart();
            npcBrain.isBusy = true;
        }

        protected override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
