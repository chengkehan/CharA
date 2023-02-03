using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class MovingSM : ActionStateMachine
    {
        public enum Transition
        {
            Undefined = 0,
            Walk = 1, // Walking state
            Run = 2, // Running state
            WalkInjured = 3, // Injured walking state
            RunInjured = 4, // Injured running state
            StairsAscending = 5,
            StairsDescending = 6
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("Moving");
        }

        private int _walkRunBlendID = 0;
        private int walkRunBlendID
        {
            get
            {
                if (_walkRunBlendID == 0)
                {
                    _walkRunBlendID = Animator.StringToHash("WalkRunBlend");
                }
                return _walkRunBlendID;
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (GetAction() == (int)Transition.Walk)
            {
                animator.SetFloat(walkRunBlendID, Mathf.Clamp01(animator.GetFloat(walkRunBlendID) - 4 * Time.deltaTime));
            }
            if (GetAction() == (int)Transition.Run)
            {
                animator.SetFloat(walkRunBlendID, Mathf.Clamp01(animator.GetFloat(walkRunBlendID) + 2 * Time.deltaTime));
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            animator.SetFloat(walkRunBlendID, 0);
        }
    }
}
