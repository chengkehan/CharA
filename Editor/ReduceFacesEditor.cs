#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using GameScript;

namespace GameScriptEditor
{
    public class ReduceFacesEditor : EditorWindow
    {
        [MenuItem("GameTools/Models/Reduce Faces")]
        private static void ReduceFaces()
        {
            EditorWindow.GetWindow<ReduceFacesEditor>().Show();
        }

        public Camera[] viewCameras = null;

        public GameObject[] meshTargets = null;

        public bool replaceAutomaticlly = false;

        private SerializedObject so = null;

        private SerializedProperty viewCamerasSP = null;

        private SerializedProperty meshTargetsSP = null;

        private SerializedProperty replaceAutomaticllySP = null;

        private Vector2 meshTargetsScroll = Vector2.zero;

        private const string IGNORE_NODE_NAME = "DynamicObjects";

        private void OnGUI()
        {
            if (so == null)
            {
                InitializeSerializedProperties();
            }

            EditorGUILayout.PropertyField(viewCamerasSP);
            meshTargetsScroll = EditorGUILayout.BeginScrollView(meshTargetsScroll, GUILayout.MaxHeight(100));
            {
                EditorGUILayout.PropertyField(meshTargetsSP);
            }
            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Search children"))
            {
                List<MeshFilter> children = new List<MeshFilter>();
                if (meshTargets != null)
                {
                    foreach (var meshTarget in meshTargets)
                    {
                        if (meshTarget != null)
                        {
                            children.AddRange(meshTarget.GetComponentsInChildren<MeshFilter>());
                        }
                    }
                    meshTargets = children.Select(r => r.gameObject).ToArray();
                }
                InitializeSerializedProperties();
            }
            EditorGUILayout.PropertyField(replaceAutomaticllySP);

            if (GUILayout.Button("Start Reduce Faces"))
            {
                if (IsArrayEmpty(viewCameras) || IsArrayEmpty(meshTargets))
                {
                    Utils.Log("Nothing be reduces.");
                }
                else
                {
                    StartReduceFaces();
                    Utils.Log("Reduce Faces Complete.");
                }
            }
            EditorGUILayout.LabelField("Ignore all objects in " + IGNORE_NODE_NAME + " node.");
        }

        private void StartReduceFaces()
        {
            string saveFolderPath = EditorUtility.SaveFolderPanel("Save As", Application.dataPath, string.Empty);
            if (string.IsNullOrEmpty(saveFolderPath))
            {
                return;
            }

            for (int meshTargetI = 0; meshTargetI < meshTargets.Length; meshTargetI++)
            {
                EditorUtility.DisplayProgressBar("Reduce Faces", (meshTargetI + 1) +  "/" + meshTargets.Length, (float)(meshTargetI + 1) / meshTargets.Length);
                GameObject meshTarget = meshTargets[meshTargetI];
                if (meshTarget == null)
                {
                    Utils.Log("Skip a null meshTarget.");
                    continue;
                }

                MeshFilter meshFilter = meshTarget.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    Utils.Log("Skip a null meshFilter");
                    continue;
                }

                if (meshFilter.sharedMesh == null)
                {
                    Utils.Log("Skip a null mesh.");
                    continue;
                }

                if (IsInIgnoreNode(meshFilter.gameObject))
                {
                    continue;
                }

                Matrix4x4 localToWorld = meshFilter.transform.localToWorldMatrix;
                Mesh newMesh = CloneMesh(meshFilter.sharedMesh);
                Vector3[] vertices = newMesh.vertices;
                List<int> newTriangles = new List<int>();
                int[] triangles = newMesh.triangles;
                int numTriangles = triangles.Length;
                for (int triangleI = 0; triangleI < numTriangles; triangleI += 3)
                {
                    int i0 = triangleI + 0;
                    int i1 = triangleI + 1;
                    int i2 = triangleI + 2;
                    Vector3 p0 = vertices[triangles[i0]];
                    Vector3 p1 = vertices[triangles[i1]];
                    Vector3 p2 = vertices[triangles[i2]];
                    Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0);
                    normal = localToWorld.MultiplyVector(normal);
                    normal.Normalize();
                    bool isVisibleInCamera = false;
                    foreach (var viewCamera in viewCameras)
                    {
                        Vector3 viewCameraForward = viewCamera.transform.forward;
                        Vector3 viewCameraBackward = -viewCamera.transform.forward;
                        if (Vector3.Dot(normal, viewCameraBackward) > -0.1f)
                        {
                            isVisibleInCamera = true;
                            break;
                        }
                    }
                    if (isVisibleInCamera)
                    {
                        newTriangles.Add(triangles[i0]);
                        newTriangles.Add(triangles[i1]);
                        newTriangles.Add(triangles[i2]);
                    }
                }
                newMesh.triangles = newTriangles.ToArray();
                newMesh.UploadMeshData(true);

                string assetPath = saveFolderPath + "/" + newMesh.name + ".asset";
                assetPath = "Assets" + assetPath.Substring(Application.dataPath.Length);
                AssetDatabase.CreateAsset(newMesh, assetPath);
                AssetDatabase.ImportAsset(assetPath);

                if (replaceAutomaticllySP.boolValue)
                {
                    meshFilter.sharedMesh = AssetDatabase.LoadMainAssetAtPath(assetPath) as Mesh;
                    EditorUtility.SetDirty(meshFilter);
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private bool IsInIgnoreNode(GameObject go)
        {
            Transform target = go == null ? null : go.transform;
            while (true)
            {
                if (target == null)
                {
                    return false;
                }
                if (target.name == IGNORE_NODE_NAME)
                {
                    return true;
                }
                target = target.parent;
            }
            return false;
        }

        private bool IsArrayEmpty<T>(T[] array)
        {
            if (array == null || array.Length == 0)
            {
                return true;
            }
            else
            {
                foreach (var item in array)
                {
                    if (item != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private Mesh CloneMesh(Mesh mesh)
        {
            Mesh newMesh = new Mesh();
            newMesh.name = mesh.name;
            newMesh.vertices = mesh.vertices;
            newMesh.normals = mesh.normals;
            newMesh.colors = mesh.colors;
            newMesh.uv = CloneUV(mesh.uv);
            newMesh.uv2 = CloneUV(mesh.uv2);
            newMesh.uv3 = CloneUV(mesh.uv3);
            newMesh.uv4 = CloneUV(mesh.uv4);
            newMesh.uv5 = CloneUV(mesh.uv5);
            newMesh.uv6 = CloneUV(mesh.uv6);
            newMesh.uv7 = CloneUV(mesh.uv7);
            newMesh.uv8 = CloneUV(mesh.uv8);
            newMesh.triangles = mesh.triangles;
            newMesh.bounds = mesh.bounds;
            newMesh.UploadMeshData(false);

            return newMesh;
        }

        private Vector2[] CloneUV(Vector2[] uv)
        {
            if (uv == null || uv.Length == 0)
            {
                return null;
            }
            else
            {
                return uv;
            }
        }

        private void InitializeSerializedProperties()
        {
            ScriptableObject target = this;
            so = new SerializedObject(target);
            viewCamerasSP = so.FindProperty("viewCameras");
            meshTargetsSP = so.FindProperty("meshTargets");

            SerializedProperty _replaceAutomaticllySP = replaceAutomaticllySP;
            replaceAutomaticllySP = so.FindProperty("replaceAutomaticlly");
            if (_replaceAutomaticllySP != null)
            {
                replaceAutomaticllySP.boolValue = _replaceAutomaticllySP.boolValue;
            }
        }
    }
}
#endif