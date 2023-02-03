using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    public class LadderMotionOperation : BaseOperation
    {
        private Waypoint _startWaypoint = null;

        public void SetStartWaypoint(Waypoint waypoint)
        {
            _startWaypoint = waypoint;
        }

        public Waypoint GetStartWaypoint()
        {
            return _startWaypoint;
        }

        public override bool ExecuteOperation()
        {
            if (GetRoleAnimation().GetWaypointPath().SearchWaypointPath(GetStartWaypoint(), GetEndWaypoint()))
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