using System.Collections.Generic;
using UnityEngine;
using GameScript.Cutscene;

namespace GameScript.WaypointSystem
{
    public class WaypointPath
    {
        private List<Waypoint> pathOfSearchingResult = new List<Waypoint>();

        private List<Waypoint> originalPathOfSearchingResult = new List<Waypoint>();

        private Waypoint _targetWaypoint = null;

        private RoleAnimation _roleAnimation = null;

        private bool _isArrivedTargetWaypoint = false;

        public WaypointPath(RoleAnimation roleAnimation)
        {
            _roleAnimation = roleAnimation;
        }

        public RoleAnimation GetRoleAnimation()
        {
            return _roleAnimation;
        }

        public bool IsApproximateTurning90(Vector3 v1, Vector3 v2)
        {
            v1.Normalize();
            v2.Normalize();
            float degree = Mathf.Abs(Vector3.Dot(v1, v2));
            return degree > -0.1f && degree < 0.1f;
        }

        public void SetTargetWaypoint(Waypoint waypoint)
        {
            _isArrivedTargetWaypoint = false;
            _targetWaypoint = waypoint;
            if (_targetWaypoint != null)
            {
                _targetWaypoint.SetGizmosColor(Color.green);
            }
        }

        public Waypoint GetTargetWaypoint()
        {
            return _targetWaypoint;
        }

        public void SetIsArrivedTargetWaypoint(bool isArrived)
        {
            _isArrivedTargetWaypoint = isArrived;
        }

        // This value is not updated/correct when climbing ladder
        // So don't access this value when climbing ladder
        public bool GetIsArrivedTargetWaypoint()
        {
            return _isArrivedTargetWaypoint;
        }

        public Waypoint GetFirstWaypointOfSearchingResult()
        {
            if (pathOfSearchingResult.Count > 0)
            {
                return pathOfSearchingResult[0];
            }
            else
            {
                return null;
            }
        }

        public Waypoint GetWaypointInOriginalSearchingResult(int index)
        {
            if (index < 0 || index >= originalPathOfSearchingResult.Count)
            {
                return null;
            }
            return originalPathOfSearchingResult[index];
        }

        public Waypoint GetTheNextWaypointInOriginalSearchingResult(Waypoint referenceWaypoint)
        {
            if (referenceWaypoint == null)
            {
                return null;
            }

            int index = originalPathOfSearchingResult.IndexOf(referenceWaypoint);
            if (index == -1)
            {
                return null;
            }

            ++index;
            if (index < 0 || index >= originalPathOfSearchingResult.Count)
            {
                return null;
            }

            return originalPathOfSearchingResult[index];
        }

        public Waypoint GetThePreviousWaypointInOriginalSearchingResult(Waypoint referenceWaypoint)
        {
            if (referenceWaypoint == null)
            {
                return null;
            }

            int index = originalPathOfSearchingResult.IndexOf(referenceWaypoint);
            if (index == -1)
            {
                return null;
            }

            --index;
            if (index < 0 || index >= originalPathOfSearchingResult.Count)
            {
                return null;
            }

            return originalPathOfSearchingResult[index];
        }

        public bool ContainsDoorInSearchingResult(Door door)
        {
            if (pathOfSearchingResult == null)
            {
                return false;
            }

            foreach (var waypoint in pathOfSearchingResult)
            {
                if (waypoint != null && waypoint.IsDoor() && waypoint.door == door)
                {
                    return true;
                }
            }
            return false;
        }

        public int NumberWaypointsOfSearchingResult()
        {
            return pathOfSearchingResult.Count;
        }

        public int NumberWaypointsOfOriginalSearchingResult()
        {
            return originalPathOfSearchingResult.Count;
        }

        public void ClearSearchingResult()
        {
            pathOfSearchingResult.Clear();
            originalPathOfSearchingResult.Clear();
        }

        public void DrawGizmosOfSearchingResult()
        {
            for (int waypointI = 0; waypointI < Waypoint.NumberWaypoints(); waypointI++)
            {
                Waypoint.GetWaypoint(waypointI).ResetGizmosColor();
            }

            for (int i = 0; i < pathOfSearchingResult.Count; i++)
            {
                pathOfSearchingResult[i].SetGizmosColor(new Color((float)(i + 1) / pathOfSearchingResult.Count, 0, 0));
            }
        }

        // If all remained waypoints are in oppisite direction, it means they are not redundant, so don't cut them.
        public bool CheckWhetherToCutOppisiteDirectionWaypointsOfSeatchingResult(Vector3 referenceDirection, Vector3 referencePosition)
        {
            referenceDirection.Normalize();

            int count = 0;
            for (int i = 0; i < pathOfSearchingResult.Count; i++)
            {
                Waypoint waypoint = pathOfSearchingResult[i];

                Vector3 v = waypoint.GetPosition() - referencePosition;
                v.Normalize();

                if (Vector3.Dot(v, referenceDirection) < 0)
                {
                    ++count;
                }
            }

            return count != pathOfSearchingResult.Count;
        }

        public void CutOppisiteDirectionWaypointsOfSearchingResult(Vector3 referenceDirection, Vector3 referencePosition)
        {
            referenceDirection.Normalize();

            for (int i = 0; i < pathOfSearchingResult.Count; i++)
            {
                Waypoint waypoint = pathOfSearchingResult[i];

                Vector3 v = waypoint.GetPosition() - referencePosition;
                v.Normalize();

                if (Vector3.Dot(v, referenceDirection) < 0)
                {
                    pathOfSearchingResult.RemoveAt(i);
                    --i;
                }
                else
                {
                    break;
                }
            }
        }

        public void CutFirstWaypointOfSearchingResult()
        {
            if (pathOfSearchingResult.Count > 0)
            {
                pathOfSearchingResult.RemoveAt(0);
            }
        }

        public void OptimizeSearchingResult()
        {
            Waypoint theFirstWaypoint = pathOfSearchingResult.Count > 0 ? pathOfSearchingResult[0] : null;
            for (int i = 0; i < pathOfSearchingResult.Count - 1/*always keep the last one*/; i++)
            {
                if (pathOfSearchingResult[i].type == Waypoint.Type.Moving)
                {
                    bool shouldBeRemoved = true;
                    // the first moving type waypoint always should be removed to keep face direciton detection correctly.
                    if (pathOfSearchingResult[i] == theFirstWaypoint)
                    {
                        shouldBeRemoved = true;
                    }
                    else
                    {
                        if (pathOfSearchingResult[i].isImportant || pathOfSearchingResult[i].isStair ||
                            pathOfSearchingResult[i].IsDoor() || pathOfSearchingResult[i].IsPortal())
                        {
                            shouldBeRemoved = false;
                        }
                        else
                        {
                            for (int neighbourI = 0; neighbourI < pathOfSearchingResult[i].NumberNeighbours(); neighbourI++)
                            {
                                var neighbour = pathOfSearchingResult[i].GetNeighbour(neighbourI);
                                if (neighbour != null &&
                                    (
                                        neighbour.type != Waypoint.Type.Moving || /*Retain waypoints beside non-moving-type-waypoints*/
                                        neighbour.IsDoor() /*Retain waypoints beside door type waypoint*/
                                    )
                                   )
                                {
                                    shouldBeRemoved = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (shouldBeRemoved)
                    {
                        pathOfSearchingResult.RemoveAt(i);
                        --i;
                    }
                }
            }
        }

        public Waypoint GetNearestMovingWaypointInBounds(Vector3 referencePosition, Bounds localBounds, Matrix4x4 worldToLocalBounds)
        {
            Waypoint nearestWaypoint = null;
            float nearestDistance = float.MaxValue;
            for (int waypointI = 0; waypointI < Waypoint.NumberWaypoints(); waypointI++)
            {
                var waypoint = Waypoint.GetWaypoint(waypointI);
                if (waypoint.type == Waypoint.Type.Moving &&
                    waypoint.IsDoor() == false/*we can't stop on a door type waypoint*/)
                {
                    var waypointPosition = waypoint.GetPosition();
                    waypointPosition = worldToLocalBounds.MultiplyPoint(waypointPosition);
                    if (localBounds.Contains(waypointPosition))
                    {
                        float distance = Vector3.SqrMagnitude(referencePosition - waypoint.GetPosition());
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestWaypoint = waypoint;
                        }
                    }
                }
            }
            return nearestWaypoint;
        }

        public Waypoint GetNearestMovingWaypoint(Vector3 referencePosition)
        {
            return GetNearestMovingWaypointInBounds(referencePosition, new Bounds(Vector3.zero, Vector3.one * 999999), Matrix4x4.identity);
        }

        public bool SearchWaypointPath(Waypoint startWaypoint, Waypoint endWaypoint)
        {
            if (startWaypoint == null || endWaypoint == null)
            {
                return false;
            }

            for (int waypointI = 0; waypointI < Waypoint.NumberWaypoints(); waypointI++)
            {
                Waypoint.GetWaypoint(waypointI).parentForPathSearching = null;
            }

            startWaypoint.parentForPathSearching = startWaypoint;

            List<Waypoint> searchingList = new List<Waypoint>();
            searchingList.Add(startWaypoint);

            List<Waypoint> waitingList = new List<Waypoint>();

            bool interruptSearching = false;

            while (searchingList.Count > 0)
            {
                for (int searchingI = 0; searchingI < searchingList.Count; searchingI++)
                {
                    Waypoint searchingWaypoing = searchingList[searchingI];
                    for (int neighbourI = 0; neighbourI < searchingWaypoing.NumberNeighbours(); neighbourI++)
                    {
                        var neighbour = searchingWaypoing.GetNeighbour(neighbourI);
                        if (neighbour != null && neighbour.parentForPathSearching == null && neighbour.type != Waypoint.Type.Hang)
                        {
                            neighbour.parentForPathSearching = searchingWaypoing;
                            waitingList.Add(neighbour);

                            if (neighbour == endWaypoint)
                            {
                                interruptSearching = true;
                                break;
                            }
                        }
                    }
                }

                if (interruptSearching)
                {
                    break;
                }

                searchingList.Clear();
                searchingList.AddRange(waitingList);
                waitingList.Clear();
            }

            pathOfSearchingResult.Clear();
            startWaypoint.parentForPathSearching = null;
            Waypoint aWaypoint = endWaypoint;
            if (aWaypoint.parentForPathSearching != null) // StartWaypoint and EndWaypoint is connected
            {
                // Construct searching path
                while (aWaypoint.parentForPathSearching != null)
                {
                    pathOfSearchingResult.Add(aWaypoint);
                    aWaypoint = aWaypoint.parentForPathSearching;
                }
            }
            else // StartWaypoint and EndWaypoint is disconnected
            {
                // Find a nearest waypoint from EndWaypoint that we searched before,
                // use it as a new EndWaypoint.
                float minDistance = 999999;
                Waypoint nearestWaypoint = null;
                for (int waypointI = 0; waypointI < Waypoint.NumberWaypoints(); waypointI++)
                {
                    var waypoint = Waypoint.GetWaypoint(waypointI);
                    if (waypoint.parentForPathSearching != null && waypoint != endWaypoint)
                    {
                        float distance = Vector3.Distance(waypoint.GetPosition(), endWaypoint.GetPosition());
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestWaypoint = waypoint;
                        }
                    }
                }
                if (nearestWaypoint != null)
                {
                    // Check whether StartWaypoint is more nearer than new EndWaypoint
                    if (Vector3.Distance(startWaypoint.GetPosition(), endWaypoint.GetPosition()) < minDistance)
                    {
                        // Do nothing
                        // Searching path is empty
                    }
                    else
                    {
                        // Construct searching path with new EndWaypoint
                        while (nearestWaypoint.parentForPathSearching != null)
                        {
                            pathOfSearchingResult.Add(nearestWaypoint);
                            nearestWaypoint = nearestWaypoint.parentForPathSearching;
                        }
                    }
                }
            }
            pathOfSearchingResult.Add(startWaypoint);

            // Inverse waypoints
            // Searching result is from end to start, we should inverse it, make it from start to end.
            for (int i = 0; i < Mathf.FloorToInt(pathOfSearchingResult.Count * 0.5f); i++)
            {
                Waypoint tempWaypoint = pathOfSearchingResult[i];
                pathOfSearchingResult[i] = pathOfSearchingResult[pathOfSearchingResult.Count - 1 - i];
                pathOfSearchingResult[pathOfSearchingResult.Count - 1 - i] = tempWaypoint;
            }

            originalPathOfSearchingResult.Clear();
            originalPathOfSearchingResult.AddRange(pathOfSearchingResult);

            return pathOfSearchingResult.Count > 0;
        }
    }
}
