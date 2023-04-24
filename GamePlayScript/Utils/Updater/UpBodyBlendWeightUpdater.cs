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

        private float speed = 2.5f;

        public UpBodyBlendWeightUpdater(Animator animator, bool isBlendIn, int layerIndex)
        {
            Utils.Assert(animator != null);

            this.animator = animator;
            this.isBlendIn = isBlendIn;
            this.layerIndex = layerIndex;

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
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
