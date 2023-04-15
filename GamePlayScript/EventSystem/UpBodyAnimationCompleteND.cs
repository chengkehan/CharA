using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class UpBodyAnimationCompleteND : NotificationData
    {
        public string roleId = null;

        public MotionAnimator.UpBodyAnimation upBodyAnimation = MotionAnimator.UpBodyAnimation.None;

        public MotionAnimator.UpBodyAnimationLayer2 upBodyAnimation2 = MotionAnimator.UpBodyAnimationLayer2.None;

        public void Reset()
        {
            upBodyAnimation = MotionAnimator.UpBodyAnimation.None;
            upBodyAnimation2 = MotionAnimator.UpBodyAnimationLayer2.None;
        }
    }
}
