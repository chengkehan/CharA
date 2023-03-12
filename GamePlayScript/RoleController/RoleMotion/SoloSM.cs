using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class SoloSM : ActionStateMachine
    {
        public enum Transition
        {
            Undefined = 0,
            StandUp = 1,
            StandingToCrouched = 2,
            CrouchedToStanding = 3,
            SittingGroundDown = 4,
            SittingGround = 5,
            SittingGroundUp = 6
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("Solo");
        }

        protected override void InitializeSequenceActions()
        {
            base.InitializeSequenceActions();

            AddSequenceAction(Transition.SittingGroundDown, Transition.SittingGround);
        }
    }
}
