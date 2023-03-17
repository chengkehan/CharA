using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    public class JumpDownSM : ActionStateMachine
    {
        public enum Transition
        {
            Undefined = 0,
            JumpDownFar = 1, // Jump down from a platform and droppoint is a far place from that platform in horizontal.
            JumpDownNear = 2, // Jump down from a platform and droppoint is a near place from that platform in horizontal.
            JumpDownFarHigh = 3, // Jump down from a high platform and droppoint is a far place from that platform in horizontal.
            JumpDownFarHighMirrored = 4, // The same as JumpDownFarHigh, just mirror animation.
            JumpDownNearHigh = 5, // Jump down from a high platform and droppoint is a near place from that platform in horizontal.
            JumpDownNearHighMirrored = 6, // The same as JumpDownNearHigh, just mirror animation.
            JumpDownInjured = 7, // Be injured and jump down
            JumpDownInjuredMirrored = 8, // The same as JumpDownInjured, just mirror animation.
            JumpDownFarInjured = 9, // The same as JumpDownFar, InjuredFall additively.
            JumpDownNearInjured = 10, // The same as JumpDownNear, InjuredFall additively.
            JumpDownFarHighInjured = 11, // The same as JumpDownFarHigh, InjuredFall additively.
            JumpDownFarHighMirroredInjured = 12, // The same as JumpDownFarHighMirrored, InjuredFall additively.
            JumpDownNearHighInjured = 13, // The same as JumpDownNearHigh, InjuredFall additively.
            JumpDownNearHighMirroredInjured = 14, // The same as JumpDownNearHighMirrored, InjuredFall additively
            JumpDownHardLanding = 15,
            JumpDownStandup = 31,
            JumpDownInjuredFall = 91,
            JumpDownHardLandingStandup = 151
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("JumpDown");
        }

        protected override int GetAction(string clipName)
        {
            return Utils.EnumToValue(Utils.StringToEnum<Transition>(clipName));
        }

        protected override void InitializeCompleteTimeOfActions()
        {
            AddCompleteTimeOfAction(Transition.JumpDownFarHigh, 0.9f);
            AddCompleteTimeOfAction(Transition.JumpDownFarHighMirrored, 0.9f);
            AddCompleteTimeOfAction(Transition.JumpDownNearHigh, 0.9f);
            AddCompleteTimeOfAction(Transition.JumpDownNearHighMirrored, 0.9f);
            AddCompleteTimeOfAction(Transition.JumpDownInjured, 0.8f);
            AddCompleteTimeOfAction(Transition.JumpDownInjuredMirrored, 0.8f);
            AddCompleteTimeOfAction(Transition.JumpDownFarInjured, 0.5f);
            AddCompleteTimeOfAction(Transition.JumpDownNearInjured, 0.5f);
            AddCompleteTimeOfAction(Transition.JumpDownFarHighInjured, 0.75f);
            AddCompleteTimeOfAction(Transition.JumpDownFarHighMirroredInjured, 0.75f);
            AddCompleteTimeOfAction(Transition.JumpDownNearHighInjured, 0.7f);
            AddCompleteTimeOfAction(Transition.JumpDownNearHighMirroredInjured, 0.7f);
            AddCompleteTimeOfAction(Transition.JumpDownStandup, 0.5f);
            AddCompleteTimeOfAction(Transition.JumpDownHardLanding, 0.70f);
        }

        protected override void InitializeSequenceActions()
        {
            AddSequenceAction(Transition.JumpDownFarHigh, Transition.JumpDownStandup);
            AddSequenceAction(Transition.JumpDownFarHighMirrored, Transition.JumpDownStandup);
            AddSequenceAction(Transition.JumpDownNearHigh, Transition.JumpDownStandup);
            AddSequenceAction(Transition.JumpDownNearHighMirrored, Transition.JumpDownStandup);
            AddSequenceAction(Transition.JumpDownFarInjured, Transition.JumpDownInjuredFall);
            AddSequenceAction(Transition.JumpDownNearInjured, Transition.JumpDownInjuredFall);
            AddSequenceAction(Transition.JumpDownFarHighInjured, Transition.JumpDownInjuredFall);
            AddSequenceAction(Transition.JumpDownFarHighMirroredInjured, Transition.JumpDownInjuredFall);
            AddSequenceAction(Transition.JumpDownNearHighInjured, Transition.JumpDownInjuredFall);
            AddSequenceAction(Transition.JumpDownNearHighMirroredInjured, Transition.JumpDownInjuredFall);
            AddSequenceAction(Transition.JumpDownHardLanding, Transition.JumpDownHardLandingStandup);
        }

        public override void MatchTargetUpdate(Animator animator)
        {
            Waypoint groundWaypoint = GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult();
            if (groundWaypoint != null)
            {
                Waypoint hangPoint = groundWaypoint.GetAttachedHangPoint();
                if (hangPoint != null)
                {
                    groundWaypoint = hangPoint;
                }

                Vector3 matchPoint = groundWaypoint.GetPosition();

                {
                    // Match foot drop on platform
                    MatchTarget(
                        animator, Transition.JumpDownHardLanding.ToString(), AvatarTarget.Root,
                        matchPoint.x, matchPoint.y, matchPoint.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.4f, 0.66f
                    );

                    // Keep foot stand on platform
                    MatchTarget(
                        animator, Transition.JumpDownHardLandingStandup.ToString(), AvatarTarget.RightFoot,
                        0, matchPoint.y, 0, Quaternion.identity,
                        0, 1, 0, 0,
                        0.250f, 0.660f
                    );
                }

                {
                    // Match foot drop on platform
                    MatchTarget(
                        animator, Transition.JumpDownFarHigh.ToString(), AvatarTarget.Root,
                        matchPoint.x, matchPoint.y, matchPoint.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.4f, 0.66f
                    );

                    // Keep foot stand on platform
                    MatchTarget(
                        animator, Transition.JumpDownStandup.ToString(), AvatarTarget.RightFoot,
                        0, matchPoint.y, 0, Quaternion.identity,
                        0, 1, 0, 0,
                        0.230f, 0.500f
                    );
                }

                {
                    // Match foot drop on platform
                    MatchTarget(
                        animator, Transition.JumpDownNear.ToString(), AvatarTarget.Root,
                        matchPoint.x, matchPoint.y, matchPoint.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.400f, 0.600f
                    );
                }


            }
        }
    }
}
