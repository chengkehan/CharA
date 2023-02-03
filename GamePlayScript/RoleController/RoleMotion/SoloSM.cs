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
            StandUp = 1
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("Solo");
        }
    }
}
