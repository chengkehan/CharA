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
            CrouchedToStanding = 3
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("Solo");
        }
    }
}
