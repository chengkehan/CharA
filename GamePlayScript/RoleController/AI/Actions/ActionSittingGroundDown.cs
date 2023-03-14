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
            Utils.Log("OnStart");
            npcBrain.GetMotionAnimator().SetSoloState(SoloSM.Transition.SittingGroundDown);
        }

        protected override TaskStatus OnUpdate()
        {
            //npcBrain.GetMotionAnimator().SetSoloState(SoloSM.Transition.SittingGroundDown);
            //if (npcBrain.GetMotionAnimator().IsInSoloState(SoloSM.Transition.SittingGroundDown))
            //{
            //    return TaskStatus.Success;
            //}
            //else
            //{
                return TaskStatus.Continue;
            //}
        }

        protected override void OnExit()
        {
            base.OnExit();

            Utils.Log("OnExit");
        }
    }
}
