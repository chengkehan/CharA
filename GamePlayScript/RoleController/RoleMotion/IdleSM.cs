using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class IdleSM : ActionStateMachine
    {
        public enum Transition
        {
            Undefined = 0,
            NormalIdle = 1, /*NormalIdle: The most frequent animation.*/
            BoredIdle = 2, /*BoredIdle: Play this animation when there is not any operation for a very long time.*/
            BoredIdle2 = 3, /*BoredIdle2: Play this animation when there is not any operation for a very long time.*/
            LookIdle = 4, /*LookIdle: Play this animation with a low frequency after NormalIdle played.*/
            RelaxBodyIdle = 5, /*RelaxBodyIdle: Play this animation when there is not any operation for a very long time.*/
            RelaxArmIdle = 6, /*RelaxArmIdle: Play this animation when there is not any operation for a very long time with a very low frequency.*/
            StunIdle = 7, /*StunIdle: Play this animation with a very low frequency after NormalIdle played.*/
            InjuredIdle = 8, /*InjuredIdle: play this animation when man is injured.*/
            ShakeHeadNoIdle = 9,
            BoredKeepIdle = 21, /*BoredKeepIdle: Choose a random animation between BoredKeepIdle and BoredLegIdle after BoredIdle played.*/
            BoredKeepIdle2 = 211, /*BoredKeepIdle2: Transition from BoredLegIdle to BoredKeepIdle*/
            BoredLegIdle = 22, /*BoredLegIdle: Choose a random animation between BoredLegIdle and BoredKeepIdle after BoredIdle played.*/
            BoredLegIdle2 = 221 /*BoredLegIdle2: Transition from BoredKeepIdle to BoredLegIdle*/
        }

        // More higher priority more frequent played
        // Key: Transition - 1
        // Value: priority
        //private int[] priorityOfBaseIdle = new int[] {
        //    10,
        //    1,
        //    1,
        //    3,
        //    2,
        //    2,
        //    2
        //};

        // Initizlize this value at startup
        // The probability range of each one will be choosen.
        // Key: Transition - 1
        // Value: probability range
        // e.g.
        // [0.6, 0.75, 0.8, 0.95, 1]
        // Generate a random value within [0, 1), Let's say 0.85.
        // It's less than 0.95 and greater than 0.8, so we get a result that at index 3.
        //private float[] probabilityOfBaseIdle = null;

        //private float _durationTimes = 0;

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("Idle");
        }

        protected override int GetAction(string clipName)
        {
            return Utils.EnumToValue(Utils.StringToEnum<Transition>(clipName));
        }

        //public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    if (GetAction() == (int)Transition.Undefined)
        //    {
        //        SetAction(animator, (int)Transition.NormalIdle);
        //        RandomizeDurationTimes();
        //        InitizliedProbabilityOfBaseIdle();
        //    }
        //}

        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    var animStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        //    var isBaseTransition = (int)GetAction() <= NumberOfBaseIdle() && GetAction() != (int)Transition.Undefined;
        //    if (isBaseTransition && animStateInfo.normalizedTime >= GetDurationTimes() && animator.IsInTransition(layerIndex) == false)
        //    {
        //        SetAction(animator, (int)RandomizeBaseIdle());
        //        RandomizeDurationTimes();
        //    }
        //}

        //private void RandomizeDurationTimes()
        //{
        //    _durationTimes = Random.Range(1, 1);
        //}
        //private float GetDurationTimes()
        //{
        //    return _durationTimes;
        //}

        //private Transition RandomizeBaseIdle()
        //{
        //    float rnd = Random.value;
        //    for (int i = 0; i < NumberOfBaseIdle(); i++)
        //    {
        //        if (rnd < probabilityOfBaseIdle[i])
        //        {
        //            return (Transition)(i + 1);
        //        }
        //    }
        //    return Transition.NormalIdle;
        //}

        //private void InitizliedProbabilityOfBaseIdle()
        //{
        //    if (probabilityOfBaseIdle == null)
        //    {
        //        int theSumOfPriority = 0;
        //        foreach (var value in priorityOfBaseIdle)
        //        {
        //            theSumOfPriority += value;
        //        }

        //        float[] probability = new float[NumberOfBaseIdle()];
        //        for (int i = 0; i < probability.Length; i++)
        //        {
        //            int priority = priorityOfBaseIdle[i];
        //            probability[i] = (float)priority / theSumOfPriority;
        //        }

        //        probabilityOfBaseIdle = new float[NumberOfBaseIdle()];
        //        for (int i = 0; i < priorityOfBaseIdle.Length; i++)
        //        {
        //            probabilityOfBaseIdle[i] = probability[i];
        //            if (i != 0)
        //            {
        //                probabilityOfBaseIdle[i] += probabilityOfBaseIdle[i - 1];
        //            }
        //        }
        //    }
        //}

        //private int NumberOfBaseIdle()
        //{
        //    return priorityOfBaseIdle.Length;
        //}
    }
}
