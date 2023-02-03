using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript.Cutscene
{
    public class DynamicLink : SerializableMonoBehaviour<DynamicLinkPD>
    {
        public enum Operation
        {
            Link,
            Break
        }

        public Waypoint a = null;

        public Waypoint b = null;

        public Operation operation = Operation.Link;

        protected override void InitializeOnStart()
        {
            base.InitializeOnStart();

            if (pd.isLinked)
            {
                Link();
            }
            else
            {
                Break();
            }
        }

        public bool IsAOrB(Waypoint waypoint)
        {
            return waypoint == a || waypoint == b;
        }

        public void Link()
        {
            LinkWaypoints(a, b);
            LinkWaypoints(b, a);
            pd.isLinked = true;
        }

        public void Break()
        {
            BreakWaypoints(a, b);
            BreakWaypoints(b, a);
            pd.isLinked = false;
        }

        public static void LinkWaypoints(Waypoint a, Waypoint b)
        {
            if (a == null || b == null)
            {
                return;
            }

            if (a.NeighboursIndexOf(b) == -1)
            {
                bool isAdded = false;
                for (int i = 0; i < a.NumberNeighbours(); i++)
                {
                    if (a.GetNeighbour(i) == null)
                    {
                        a.SetNeighbour(b, i);
                        isAdded = true;
                        break;
                    }
                }
                if (isAdded == false)
                {
                    Utils.LogError("Link waypoints failed.");
                }
            }
        }

        public static void BreakWaypoints(Waypoint a, Waypoint b)
        {
            if (a == null || b == null)
            {
                return;
            }

            if (a.NeighboursIndexOf(b) != -1)
            {
                for (int i = 0; i < a.NumberNeighbours(); i++)
                {
                    if (a.GetNeighbour(i) == b)
                    {
                        a.SetNeighbour(null, i);
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (transform != null)
            {
                if (a != null)
                {
                    Gizmos.DrawLine(transform.position, a.GetPosition());
                }
                if (b != null)
                {
                    Gizmos.DrawLine(transform.position, b.GetPosition());
                }
            }
        }
#endif
    }
}
