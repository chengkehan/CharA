using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class SkillSM : ActionStateMachine
    {
        public enum Transition
        {
            Undefined = 0,
            Pickaxe = 1,
            Stab = 2
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("Skill");
        }
    }
}
