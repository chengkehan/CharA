#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameScript;

namespace GameScriptEditor
{
    public class CopyDBKColorWallpaperEditor : EditorWindow
    {
        [MenuItem("GameTools/Models/Copy DBK Color Wallpaper")]
        private static void CopyDBKColorWallpaper()
        {
            EditorWindow.GetWindow<CopyDBKColorWallpaperEditor>().Show();
        }

        public GameObject sourceModel = null;

        public GameObject targetModel = null;

        private SerializedObject so = null;

        private SerializedProperty sourceModelSP = null;

        private SerializedProperty targetModelSP = null;

        private void OnGUI()
        {
            if (so == null)
            {
                InitializeSerializedProperties();
            }

            EditorGUILayout.PropertyField(sourceModelSP);
            EditorGUILayout.PropertyField(targetModelSP);

            if (GUILayout.Button("Start Copy"))
            {
                if (sourceModelSP.objectReferenceValue != null && targetModelSP.objectReferenceValue != null)
                {
                    StartCopy();
                    Utils.Log("Copy DBK Color Wallpaper Complete.");
                }
            }
        }

        private void StartCopy()
        {
            var sourceDBKSettings = GatherDBKSettings(sourceModelSP.objectReferenceValue as GameObject);
            CopyDBKSettings(sourceDBKSettings, targetModelSP.objectReferenceValue as GameObject);
        }

        private void CopyDBKSettings(DBKSettingsCollection sourceDBKSettings, GameObject targetModel)
        {
            MeshRenderer[] mrs = targetModel.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in mrs)
            {
                Material mtrl = mr.sharedMaterial;
                if (mtrl != null)
                {
                    if (sourceDBKSettings.collection.TryGetValue(mtrl, out DBKSettingList aSettingsList))
                    {
                        DBKColorWallpaper aSetting = mr.gameObject.GetComponent<DBKColorWallpaper>();
                        if (aSetting == null)
                        {
                            aSetting = mr.gameObject.AddComponent<DBKColorWallpaper>();
                        }
                        EditorUtility.CopySerialized(aSettingsList.list[0], aSetting);
                        EditorUtility.SetDirty(mr);
                    }
                }
            }
        }

        private DBKSettingsCollection GatherDBKSettings(GameObject sourceModel)
        {
            var collection = new DBKSettingsCollection();
            MeshRenderer[] mrs = sourceModel.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in mrs)
            {
                Material mtrl = mr.sharedMaterial;
                DBKColorWallpaper aSetting = mr.gameObject.GetComponent<DBKColorWallpaper>();
                if (mtrl != null && aSetting != null)
                {
                    if (collection.collection.TryGetValue(mtrl, out DBKSettingList aSettingsList) == false)
                    {
                        aSettingsList = new DBKSettingList();
                        collection.collection[mtrl] = aSettingsList;
                    }
                    aSettingsList.list.Add(aSetting);
                }
            }
            return collection;
        }

        private void InitializeSerializedProperties()
        {
            ScriptableObject target = this;
            so = new SerializedObject(target);
            sourceModelSP = so.FindProperty("sourceModel");
            targetModelSP = so.FindProperty("targetModel");
        }

        private class DBKSettingsCollection
        {
            public Dictionary<Material, DBKSettingList> collection = new Dictionary<Material, DBKSettingList>();
        }

        private class DBKSettingList
        {
            public List<DBKColorWallpaper> list = new List<DBKColorWallpaper>();
        }
    }
}
#endif