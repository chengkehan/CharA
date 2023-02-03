using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    public class JumpUpSM : ActionStateMachine
    {
        public enum Transition
        {
            Undefined = 0,
            JumpUpInplace = 1,
            JumpRunForward = 2,
            JumpRunForwardMirrored = 3
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("JumpUp");
        }

        public override void MatchTargetUpdate(Animator animator)
        {
            Waypoint jumpupWaypoint =
                GetRoleAnimation().GetWaypointPath().GetThePreviousWaypointInOriginalSearchingResult
                (
                    GetRoleAnimation().GetWaypointPath().GetFirstWaypointOfSearchingResult()
                );

            if (jumpupWaypoint != null)
            {
                Waypoint hangPoint = jumpupWaypoint.GetAttachedHangPoint();
                if (hangPoint != null)
                {
                    jumpupWaypoint = hangPoint;
                }

                Vector3 matchPoint = jumpupWaypoint.GetPosition();

                {
                    // Match foot drop on platform
                    MatchTarget(
                        animator, Transition.JumpUpInplace.ToString(), AvatarTarget.Root,
                        matchPoint.x, matchPoint.y, matchPoint.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.3f, 0.55f
                    );
                }

                {
                    // Match foot drop on platform
                    MatchTarget(
                        animator, Transition.JumpRunForward.ToString(), AvatarTarget.Root,
                        matchPoint.x, matchPoint.y, matchPoint.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.1f, 1f
                    );
                }

                {
                    // Match foot drop on platform
                    MatchTarget(
                        animator, Transition.JumpRunForwardMirrored.ToString(), AvatarTarget.Root,
                        matchPoint.x, matchPoint.y, matchPoint.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.1f, 1f
                    );
                }
            }
        }
    }
}
