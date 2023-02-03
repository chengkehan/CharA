#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameScript;
using GameScript.WaypointSystem;
using GameScript.Cutscene;

namespace GameScriptEditor
{
    [CustomEditor(typeof(Waypoint)), CanEditMultipleObjects]
    public class WaypointEditor : Editor
    {
        [MenuItem("GameTools/Waypoint/Link")]
        private static void LinkTwoWaypoints2()
        {
            LinkTwoWaypoints();
        }    
        [MenuItem("GameObject/Waypoint/Link")]
        private static void LinkTwoWaypoints()
        {
            LinkWaypointNeighbour(
                Selection.gameObjects[0].GetComponent<Waypoint>(),
                Selection.gameObjects[1].GetComponent<Waypoint>()
            );
            LinkWaypointNeighbour(
                Selection.gameObjects[1].GetComponent<Waypoint>(),
                Selection.gameObjects[0].GetComponent<Waypoint>()
            );
        }
        [MenuItem("GameTools/Waypoint/Link", true)]
        private static bool LinkTwoWaypointsValidation2()
        {
            return LinkTwoWaypointsValidation();
        }
        [MenuItem("GameObject/Waypoint/Link", true)]
        private static bool LinkTwoWaypointsValidation()
        {
            var gameObjects = Selection.gameObjects;
            return gameObjects != null && gameObjects.Length == 2 &&
                    gameObjects[0] != null && gameObjects[1] != null &&
                    gameObjects[0].GetComponent<Waypoint>() != null &&
                    gameObjects[1].GetComponent<Waypoint>() != null;
        }
        [MenuItem("GameTools/Waypoint/Unlink")]
        public static void UnlinkTwoWaypoints2()
        {
            UnlinkTwoWaypoints();
        }
        [MenuItem("GameObject/Waypoint/Unlink")]
        public static void UnlinkTwoWaypoints()
        {
            UnlinkWaypointNeighbour(
                Selection.gameObjects[0].GetComponent<Waypoint>(),
                Selection.gameObjects[1].GetComponent<Waypoint>()
            );
            UnlinkWaypointNeighbour(
                Selection.gameObjects[1].GetComponent<Waypoint>(),
                Selection.gameObjects[0].GetComponent<Waypoint>()
            );
        }
        [MenuItem("GameTools/Waypoint/Unlink", true)]
        private static bool UnlinkTwoWaypointsValidation2()
        {
            return UnlinkTwoWaypointsValidation();
        }
        [MenuItem("GameObject/Waypoint/Unlink", true)]
        private static bool UnlinkTwoWaypointsValidation()
        {
            return LinkTwoWaypointsValidation();
        }
        private static void LinkWaypointNeighbour(Waypoint a, Waypoint b)
        {
            DynamicLink.LinkWaypoints(a, b);
            EditorUtility.SetDirty(a);
            EditorUtility.SetDirty(b);
        }
        private static void UnlinkWaypointNeighbour(Waypoint a, Waypoint b)
        {
            DynamicLink.BreakWaypoints(a, b);
            EditorUtility.SetDirty(a);
            EditorUtility.SetDirty(b);
        }

        private Waypoint waypoint = null;

        private void OnEnable()
        {
            waypoint = (Waypoint)target;
        }

        private void OnSceneGUI()
        {
            if (waypoint == null || waypoint.type == Waypoint.Type.Hang)
            {
                return;
            }

            Color color = Handles.color;

            // Z+
            OnGUI_Button(
                new Color(Color.blue.r, Color.blue.g, Color.blue.b, 1f),
                1, Vector3.forward, Waypoint.FORWARD, Waypoint.BACKWARD
            );

            // Z-
            OnGUI_Button(
                new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f),
                -1, Vector3.forward, Waypoint.BACKWARD, Waypoint.FORWARD
            );

            // X+
            OnGUI_Button(
               new Color(Color.red.r, Color.red.g, Color.red.b, 1f),
               1, Vector3.right, Waypoint.RIGHT, Waypoint.LEFT
            );

            // X-
            OnGUI_Button(
               new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f),
               -1, Vector3.right, Waypoint.LEFT, Waypoint.RIGHT
            );

            // Y+
            OnGUI_Button(
               new Color(Color.green.r, Color.green.g, Color.green.b, 1f),
               1, Vector3.up, Waypoint.TOP, Waypoint.BOTTOM
            );

            // Y-
            OnGUI_Button(
               new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f),
               -1, Vector3.up, Waypoint.BOTTOM, Waypoint.TOP
            );

            Handles.color = color;
        }

        private void OnGUI_Button(Color color, int flag, Vector3 directionV3, int direction, int inverseDirection)
        {
            Handles.color = color;
            if (Handles.Button(waypoint.transform.position + directionV3 * 0.35f * flag, Quaternion.LookRotation(directionV3 * -flag), 0.1f, 0.1f, Handles.CubeHandleCap))
            {
                if (waypoint.GetNeighbour(direction) == null)
                {
                    var go = CreateNeighbourWaypoint(direction, inverseDirection, directionV3 * 0.5f * flag);
                    Selection.activeGameObject = go;
                }
                else
                {
                    Selection.activeGameObject = waypoint.GetNeighbour(direction).gameObject;
                }
            }
        }

        private GameObject CreateNeighbourWaypoint(int direction, int inverseDirection, Vector3 positionOffset)
        {
            GameObject go = new GameObject();
            Waypoint wp = go.AddComponent<Waypoint>();
            waypoint.SetNeighbour(wp, direction);
            wp.SetNeighbour(waypoint, inverseDirection);
            go.transform.parent = waypoint.transform.parent;
            go.transform.localPosition = waypoint.transform.localPosition + positionOffset;
            EditorUtility.SetDirty(waypoint);

            return go;
        }
    }
}
#endif