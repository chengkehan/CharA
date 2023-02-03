using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    public class MovingOperation : BaseOperation
    {
        public override bool ExecuteOperation()
        {
            Waypoint startWaypoint = GetRoleAnimation().GetWaypointPath().GetNearestMovingWaypoint(GetRoleAnimation().GetMotionAnimator().GetPosition());
            if (GetRoleAnimation().GetWaypointPath().SearchWaypointPath(startWaypoint, GetEndWaypoint()))
            {
                GetRoleAnimation().GetWaypointPath().OptimizeSearchingResult();
                GetRoleAnimation().GetWaypointPath().DrawGizmosOfSearchingResult();
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void OnSecondOperationUpdate()
        {
            // Do nothing
        }

        public override void OnSecondOperationTheEnd()
        {
            // Do nothing
        }
    }
}