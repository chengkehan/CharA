using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.Cutscene;

namespace GameScript.WaypointSystem
{
    public class Waypoint : MonoBehaviour
    {
        public enum Type
        {
            // Interactive, Remove If Redundant.
            // Noninteractive and nonremoveable if IsDoor.
            // Nonremoveable if IsPortal
            Moving = 1,
            Turning = 2, // Slow down speed if turning 
            Climbing = 3,
            JumpDown = 4,
            Hang = 5, // To indicate a reference point for matching target(hand, foot, etc.)
            JumpUp = 6,
            Ladder = 7 // Interactive
        }

        [SerializeField]
        private Type _type = Type.Moving;
        public Type type
        {
            get
            {
                return _type;
            }
        }

        [SerializeField]
        private bool _isImportant = false;
        public bool isImportant
        {
            get
            {
                return _isImportant;
            }
        }

        // If isStair, it must be isImportant
        [SerializeField]
        private bool _isStair = false;
        public bool isStair
        {
            get
            {
                return _isStair;
            }
        }

        [Tooltip("When role arrives this point, role will be port to specfied waypoint.\nLeave this empty if it's not a portal.")]
        [SerializeField]
        private Waypoint _portToWaypoint = null;
        public Waypoint portToWaypoint
        {
            get
            {
                return _portToWaypoint;
            }
        }

        [SerializeField]
        private Door _door = null;
        public Door door
        {
            get
            {
                return _door;
            }
        }

        private static List<Waypoint> _allWaypoints = new List<Waypoint>();
        private static void ResetWaypoints()
        {
            _allWaypoints.Clear();
        }
        private static void AddWaypoint(Waypoint p)
        {
            _allWaypoints.Add(p);
        }
        public static int NumberWaypoints()
        {
            return _allWaypoints.Count;
        }
        public static Waypoint GetWaypoint(int index)
        {
            if (WaypointIndexOutOfRange(index))
            {
                return null;
            }
            else
            {
                return _allWaypoints[index];
            }
        }
        private static bool WaypointIndexOutOfRange(int index)
        {
            return index < 0 || index >= _allWaypoints.Count;
        }

        public const int LEFT = 0;
        public const int RIGHT = 1;
        public const int FORWARD = 2;
        public const int BACKWARD = 3;
        public const int TOP = 4;
        public const int BOTTOM = 5;

        [SerializeField]
        private Waypoint[] _neighbours = new Waypoint[8];
        public int NumberNeighbours()
        {
            return _neighbours.Length;
        }
        public Waypoint GetNeighbour(int index)
        {
            if (NeighbourIndexOutOfRange(index))
            {
                return null;
            }
            else
            {
                return _neighbours[index];
            }
        }
        public void SetNeighbour(Waypoint waypoint, int index)
        {
            if (NeighbourIndexOutOfRange(index) == false)
            {
                _neighbours[index] = waypoint;
            }
        }
        public int NeighboursIndexOf(Waypoint waypoint)
        {
            return System.Array.IndexOf(_neighbours, waypoint);
        }
        private bool NeighbourIndexOutOfRange(int index)
        {
            return index < 0 || index >= _neighbours.Length;
        }

        private Waypoint _parentForPathSearching = null;
        public Waypoint parentForPathSearching
        {
            set
            {
                _parentForPathSearching = value;
            }
            get
            {
                return _parentForPathSearching;
            }
        }

        private Color gizmosColor = Color.white;

        public void SetGizmosColor(Color color)
        {
            gizmosColor = color;
        }

        public Vector3 GetPosition()
        {
            return transform == null ? Vector3.zero : transform.position;
        }

        public Vector3 GetForward()
        {
            return transform == null ? Vector3.zero : transform.forward;
        }

        public bool IsPortal()
        {
            return portToWaypoint != null;
        }

        public bool IsDoor()
        {
            return door != null;
        }

        public Waypoint GetAttachedHangPoint()
        {
            for (int i = 0; i < NumberNeighbours(); i++)
            {
                if (GetNeighbour(i) != null && GetNeighbour(i).type == Type.Hang)
                {
                    return GetNeighbour(i);
                }
            }
            return null;
        }

        public void ResetGizmosColor()
        {
            gizmosColor = Color.white;
        }

        private void Awake()
        {
            AddWaypoint(this);
        }

        private void OnDestroy()
        {
            ResetWaypoints();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color gizmosColor = Gizmos.color;

            {
                if (isImportant && Application.isPlaying == false)
                {
                    Gizmos.color = Color.yellow;
                }
                else if (isStair && Application.isPlaying == false)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = this.gizmosColor;
                }
                if (type == Type.Hang)
                {
                    Gizmos.DrawCube(GetPosition(), Vector3.one * 0.15f);
                }
                else
                {
                    bool isNotInNeighbourList = false;
                    for (int i = 0; i < NumberNeighbours(); i++)
                    {
                        if (GetNeighbour(i) != null && GetNeighbour(i).NeighboursIndexOf(this) == -1)
                        {
                            isNotInNeighbourList = true;
                            break;
                        }
                    }
                    if (isNotInNeighbourList)
                    {
                        Gizmos.DrawWireSphere(GetPosition(), 0.2f);
                    }
                    else
                    {
                        Gizmos.DrawSphere(GetPosition(), 0.2f);
                    }
                }
                Gizmos.color = gizmosColor;
            }

            {
                if (type == Type.Ladder)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(GetPosition(), GetPosition() + GetForward());
                    Gizmos.color = gizmosColor;
                }
            }

            {
                for (int i = 0; i < NumberNeighbours(); i++)
                {
                    if (GetNeighbour(i) != null)
                    {
                        if (GetNeighbour(i).type == Type.Hang)
                        {
                            Gizmos.color = Color.green;
                            Vector3 p0 = GetPosition();
                            Vector3 p2 = GetNeighbour(i).GetPosition();
                            Vector3 p1 = (p0 + p2) * 0.5f + new Vector3(0, Vector3.Distance(p0, p2) * 2, 0);
                            Vector3 lineStart = GetPosition();
                            for (float t = 0; t < 1.1f; t += 0.1f)
                            {
                                Vector3 lineEnd = BezierCurve(p0, p1, p2, t);
                                Gizmos.DrawLine(lineStart, lineEnd);
                                lineStart = lineEnd;
                            }
                        }
                        else
                        {
                            Gizmos.color = gizmosColor;
                            Gizmos.DrawLine(GetPosition(), GetNeighbour(i).GetPosition());
                        }
                    }
                }

                Gizmos.color = gizmosColor;
            }

            {
                if (IsPortal())
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(GetPosition(), portToWaypoint.GetPosition());
                    Gizmos.DrawLine(portToWaypoint.GetPosition(), portToWaypoint.GetPosition() + portToWaypoint.GetForward());
                }

                Gizmos.color = gizmosColor;
            }

            {
                if (IsDoor())
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(GetPosition(), door.GetKnobPosition());
                }

                Gizmos.color = gizmosColor;
            }
        }

        private Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            t = Mathf.Clamp01(t);
            return (1 - t) * (1 - t) * p0 + 2 * t * (1 - t) * p1 + t * t * p2;
        }
#endif
    }
}
