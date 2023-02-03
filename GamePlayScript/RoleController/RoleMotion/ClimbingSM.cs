using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    public class ClimbingSM : ActionStateMachine
    {
        public enum Transition
        {
            Undefined = 0,
            ClimbingNormal = 1, // Climbing up a wall.
            ClimbingNormalMirrored = 2, // Climbing up a wall.
            ClimbingHighEasyJump = 3, // Climbing up a high wall easily.
            ClimbingHighHardJump = 4, // Climbing up a high wall hard.
            ClimbingHighFreehandJump = 5, // Climbing up a high wall freehand.
            ClimbingHighFreehandJumpHigh = 6, // Climbing up a high wall freehand.
            ClimbingUpLadderStart = 7,
            ClimbingUpLadderStart2 = 8,
            ClimbingLowNormal = 9,
            ClimbingHighEasy = 31,
            ClimbingHighHard = 41,
            ClimbingHighFreehand = 51,
            ClimbingHighFreehandHigh = 61,
            ClimbingHighEasyStandup = 32,
            ClimbingHighHardStandup = 42,
            ClimbingUpLadder = 71,
            ClimbingUpLadderEnd = 72,
            ClimbingLowNormalStandup = 91
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("Climbing");
        }

        protected override void InitializeCompleteTimeOfActions()
        {
            AddCompleteTimeOfAction(Transition.ClimbingHighFreehandJump, 0.8f);
            AddCompleteTimeOfAction(Transition.ClimbingHighEasy, 0.9f);
            AddCompleteTimeOfAction(Transition.ClimbingHighHard, 0.9f);
            AddCompleteTimeOfAction(Transition.ClimbingHighEasyStandup, 0.9f);
            AddCompleteTimeOfAction(Transition.ClimbingHighHardStandup, 0.9f);
        }

        protected override void InitializeSequenceActions()
        {
            AddSequenceAction(Transition.ClimbingHighEasyJump, Transition.ClimbingHighEasy);
            AddSequenceAction(Transition.ClimbingHighEasy, Transition.ClimbingHighEasyStandup);
            AddSequenceAction(Transition.ClimbingHighHardJump, Transition.ClimbingHighHard);
            AddSequenceAction(Transition.ClimbingHighHard, Transition.ClimbingHighHardStandup);
            AddSequenceAction(Transition.ClimbingHighFreehandJump, Transition.ClimbingHighFreehand);
            AddSequenceAction(Transition.ClimbingHighFreehandJumpHigh, Transition.ClimbingHighFreehandHigh);
            AddSequenceAction(Transition.ClimbingUpLadderStart, Transition.ClimbingUpLadder);
            AddSequenceAction(Transition.ClimbingUpLadderStart2, Transition.ClimbingUpLadder);
            AddSequenceAction(Transition.ClimbingUpLadder, Transition.ClimbingUpLadderEnd);
            AddSequenceAction(Transition.ClimbingLowNormal, Transition.ClimbingLowNormalStandup);
        }

        public override void MatchTargetUpdate(Animator animator)
        {
            Waypoint platformWaypoint = GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult();
            if (platformWaypoint != null)
            {
                Waypoint hangPoint = platformWaypoint.GetAttachedHangPoint();
                if (hangPoint != null)
                {
                    platformWaypoint = hangPoint;
                }

                Vector3 matchPoint = platformWaypoint.GetPosition();

                {
                    // Match right hand to catch edge
                    MatchTarget(
                        animator, Transition.ClimbingNormal.ToString(), AvatarTarget.RightHand,
                        matchPoint.x, matchPoint.y, matchPoint.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.000f, 0.150f
                    );
                    // Fix right hand position offset
                    MatchTarget(
                        animator, Transition.ClimbingNormal.ToString(), AvatarTarget.RightHand,
                        matchPoint.x, matchPoint.y, matchPoint.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.270f, 0.410f
                    );

                    // Match foot to stand on platform
                    MatchTarget(
                        animator, Transition.ClimbingNormal.ToString(), AvatarTarget.RightFoot,
                        0, matchPoint.y, 0, Quaternion.identity,
                        0, 1, 0, 0,
                        0.450f, 0.520f
                    );
                }

                {
                    // Match right hand to catch edge
                    MatchTarget(
                        animator, Transition.ClimbingLowNormal.ToString(), AvatarTarget.RightHand,
                        matchPoint.x, matchPoint.y, matchPoint.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.000f, 0.150f
                    );

                    // Match foot to stand on platform
                    MatchTarget(
                        animator, Transition.ClimbingLowNormal.ToString(), AvatarTarget.LeftFoot,
                        0, matchPoint.y, 0, Quaternion.identity,
                        0, 1, 0, 0,
                        0.320f, 0.620f
                    );
                }

                {
                    float halfPalmSize = 0.069f;

                    // Match right hand to catch edge
                    MatchTarget(
                        animator, Transition.ClimbingHighFreehandJumpHigh.ToString(), AvatarTarget.RightHand,
                        matchPoint.x, matchPoint.y - halfPalmSize, matchPoint.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.328f, 0.566f
                    );

                    // Match foot to stand on platform
                    MatchTarget(
                        animator, Transition.ClimbingHighFreehandHigh.ToString(), AvatarTarget.RightFoot,
                        0, matchPoint.y, 0, Quaternion.identity,
                        0, 1, 0, 0,
                        0.680f, 0.754f
                    );
                }
            }
        }
    }
}
