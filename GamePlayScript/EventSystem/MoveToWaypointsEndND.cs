using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    public class MoveToWaypointsEndND : NotificationData
    {
        public string roleID = null;

        public Waypoint endWaypoint = null;
    }
}
