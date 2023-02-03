using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class MatchTargetUtil
    {
        public static void MatchTarget(
            Animator animator, string stateName, AvatarTarget targetBodyPart,
            float matchPositionX, float matchPositionY, float matchPositionZ, Quaternion matchRotation,
            float matchPositionXWeight, float matchPositionYWeight, float matchPositionZWeight, float matchRotationWeight,
            float startNormalizedTime, float targetNormalizedTime)
        {
            if (animator != null && animator.isMatchingTarget == false && animator.IsInTransition(0) == false && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                animator.MatchTarget(
                    new Vector3(matchPositionX, matchPositionY, matchPositionZ), matchRotation, targetBodyPart,
                    new MatchTargetWeightMask(
                        new Vector3(matchPositionXWeight, matchPositionYWeight, matchPositionZWeight),
                        matchRotationWeight),
                    startNormalizedTime, targetNormalizedTime
                );
            }
        }
    }
}
