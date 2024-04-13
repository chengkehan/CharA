using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks;

namespace GameScript
{
    public class ActionSittingGroundDown : ActionBase
    {
        protected override void OnStart()
        {
            base.OnStart();
            npcBrain.GetMotionAnimator().SetSoloState(SoloSM.Transition.SittingGroundDown);
        }

        protected override TaskStatus OnUpdate()
        {
            if (npcBrain.GetMotionAnimator().IsSoloStateComplete(SoloSM.Transition.SittingGroundDown))
            {
                Utils.LogError(1);
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Continue;
            }
        }
    }
}
