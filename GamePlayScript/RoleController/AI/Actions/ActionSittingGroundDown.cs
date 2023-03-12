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
            return TaskStatus.Continue;
        }
    }
}
