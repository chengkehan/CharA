using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    public class OpenDoorSM : ActionStateMachine
    {
        public enum Transition
        {
            Undefined = 0,
            OpenDoorInwards = 1,
            OpenDoorOutwards = 2,
            OpenDoorLocked = 3,
            OpenDoorOutwardsMirror = 4,
            OpenDoorInwardsMirror = 5
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("OpenDoor");
        }
    }
}
