using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class TurningSM : ActionStateMachine
    {
        public enum Transition
        {
            Undefined = 0,
            IdleTurnLeft = 1, // Turn left when it's idling state now.
            IdleTurnRight = 2, // Turn right when it's idling state now.
            WalkTurnLeft = 3, // Turn left when it's walking state now.
            WalkTurnRight = 4, // Turn right when it's walking state now.
            RunTurnLeft = 5, // Turn left when it's running state now.
            RunTurnRight = 6, // Turn right when it's running state now.
            InjuredTurnLeft = 7, // Turn left when man is injured
            InjuredTurnRight = 8, // Turn right when man is injured
            IdleTurnLeft90 = 9,
            IdleTurnRight90 = 10,
            WalkTurnLeft90 = 11,
            WalkTurnRight90 = 12,
            RunTurnLeft90 = 13,
            RunTurnRight90 = 14
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("Turning");
        }

        protected override void InitializeCompleteTimeOfActions()
        {
            AddCompleteTimeOfAction(Transition.InjuredTurnLeft, 2f);
            AddCompleteTimeOfAction(Transition.InjuredTurnRight, 2f);
            AddCompleteTimeOfAction(Transition.IdleTurnLeft90, 0.5f);
            AddCompleteTimeOfAction(Transition.IdleTurnRight90, 0.5f);
            AddCompleteTimeOfAction(Transition.WalkTurnLeft90, 0.4f);
            AddCompleteTimeOfAction(Transition.WalkTurnRight90, 0.4f);
            AddCompleteTimeOfAction(Transition.RunTurnLeft90, 0.4f);
            AddCompleteTimeOfAction(Transition.RunTurnRight90, 0.4f);
        }
    }
}
