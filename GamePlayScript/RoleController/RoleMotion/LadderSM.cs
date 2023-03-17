using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    public class LadderSM : ActionStateMachine
    {
        public enum Transition
        {
            Undefined = 0,
            LadderUpStart = 1,
            LadderUpClimbing = 2,
            LadderUpEnd = 3,
            LadderDownStart = 4,
            LadderDownClimbing = 5,
            LadderDownEnd = 6
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("Ladder");
        }

        protected override int GetAction(string clipName)
        {
            return Utils.EnumToValue(Utils.StringToEnum<Transition>(clipName));
        }

        public override void MatchTargetUpdate(Animator animator)
        {
            Waypoint targetWaypoint = GetRoleAnimation().GetWaypointPath().GetTargetWaypoint();
            if (targetWaypoint != null)
            {
                Vector3 targetPosition = targetWaypoint.GetPosition();

                Waypoint targetHand = targetWaypoint.GetAttachedHangPoint();
                Vector3 targetHandPosition = Vector3.zero;
                if (targetHand == null)
                {
                    targetHandPosition = targetPosition;
                }
                else
                {
                    targetHandPosition = targetHand.GetPosition();
                }

                {
                    MatchTarget(
                        animator, Transition.LadderUpStart.ToString(), AvatarTarget.Root,
                        targetPosition.x, targetPosition.y, targetPosition.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.15f, 1
                    );

                    MatchTarget(
                        animator, Transition.LadderUpEnd.ToString(), AvatarTarget.LeftFoot,
                        0, targetHandPosition.y, 0, Quaternion.identity,
                        0, 1, 0, 0,
                        0.395f, 0.513f
                    );

                    MatchTarget(
                        animator, Transition.LadderUpEnd.ToString(), AvatarTarget.Root,
                        targetHandPosition.x, targetHandPosition.y, targetHandPosition.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.55f, 0.85f
                    );
                }

                {
                    MatchTarget(
                        animator, Transition.LadderDownStart.ToString(), AvatarTarget.Root,
                        targetPosition.x, targetPosition.y, targetPosition.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0.45f, 1
                    );

                    MatchTarget(
                        animator, Transition.LadderDownEnd.ToString(), AvatarTarget.Root,
                        targetHandPosition.x, targetHandPosition.y, targetHandPosition.z, Quaternion.identity,
                        1, 1, 1, 0,
                        0, 1
                    );
                }
            }
        }
    }
}
