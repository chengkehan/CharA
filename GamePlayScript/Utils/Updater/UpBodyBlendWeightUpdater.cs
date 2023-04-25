using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class UpBodyBlendWeightUpdater : IUpdater
    {
        private Animator animator = null;

        private bool isBlendIn = false;

        private int layerIndex = 0;

        private float weight = 0;

        private float speed = 2f;

        private UpBodySMBase upBodySM = null;

        private int actionValue = 0;

        public UpBodyBlendWeightUpdater(UpBodySMBase upBodySM, Animator animator, int actionValue, bool isBlendIn, int layerIndex)
        {
            Utils.Assert(animator != null);
            Utils.Assert(upBodySM != null);

            this.upBodySM = upBodySM;
            this.animator = animator;
            this.isBlendIn = isBlendIn;
            this.layerIndex = layerIndex;
            this.actionValue = actionValue;

            weight = animator.GetLayerWeight(layerIndex);
        }

        public bool Update()
        {
            if (animator == null)
            {
                return false;
            }

            if (isBlendIn)
            {
                weight += speed * Time.deltaTime;
                animator.SetLayerWeight(layerIndex, Mathf.Clamp01(weight));
            }
            else
            {
                weight -= speed * Time.deltaTime;
                animator.SetLayerWeight(layerIndex, Mathf.Clamp01(weight));
            }

            if (weight <= 0 || weight >= 1)
            {
                if (actionValue == 0)
                {
                    upBodySM.SetAction(animator, actionValue);
                }

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
