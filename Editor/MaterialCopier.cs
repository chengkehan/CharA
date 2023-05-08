#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using GameScript;
using System.Linq;

namespace GameScriptEditor
{
    public class MaterialCopier
    {
        private static Material mat = null;

        private static Texture2D albedoTexture = null;

        private static Texture2D normalTexture = null;

        [MenuItem("Assets/Auto Clone Material", false, 0)]
        [MenuItem("GameTools/Material/Auto Clone Material")]
        private static void AutoCloneMaterial()
        {
            var selectedObjs = Selection.objects;
            foreach (var item in selectedObjs)
            {
                if (item is Material)
                {
                    bool matched = false;
                    string[] matchingAssets = AssetDatabase.FindAssets(item.name + " t:Material");
                    foreach (var matchingItem in matchingAssets)
                    {
                        var mtrl = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(matchingItem)) as Material;
                        if (mtrl.shader != Shader.Find("Universal Render Pipeline/Lit"))
                        {
                            Selection.activeObject = mtrl;
                            DoRecord();
                            Selection.activeObject = item;
                            DoApply();
                            matched = true;
                            break;
                        }
                    }
                    EditorUtility.SetDirty(item); 
                    if (matched == false)
                    {
                        Utils.LogObservably("Mismatching material, " + item.name);
                    }
                }
            }
        }
        [MenuItem("Assets/Auto Clone Material", true, 0)]
        [MenuItem("GameTools/Material/Auto Clone Material", true)]
        private static bool AutoCloneMaterialValidation()
        {
            foreach (var item in Selection.objects)
            {
                if (item is Material)
                {
                    return true;
                }
            }
            return false;
        }

        [MenuItem("Assets/Preprocess UE Assets", false, 0)]
        [MenuItem("GameTools/Material/Preprocess UE Assets")]
        private static void PreprocessUEAssets()
        {
            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var dirInfo = new FileInfo(assetPath).Directory;
            var dirPath = FileUtil.GetProjectRelativePath(dirInfo.FullName);
            ExtractMaterials(assetPath, dirPath);
            var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            var assetGo = GameObject.Instantiate(asset) as GameObject;
            assetGo.name = asset.name;
            Object.DestroyImmediate(assetGo.GetComponent<LODGroup>());
            EditorUtility.SetDirty(assetGo); 
            var deletedList = new List<Transform>();
            AddToDeletedList(assetGo.transform, deletedList);
            foreach (var deletedItem in deletedList)
            {
                Object.DestroyImmediate(deletedItem.gameObject);
            }
            var prefabPath = dirPath + "/" + assetGo.name + ".prefab";
            PrefabUtility.SaveAsPrefabAssetAndConnect(assetGo, prefabPath, InteractionMode.AutomatedAction);
            AssetDatabase.ImportAsset(prefabPath);
        }

        [MenuItem("Assets/Preprocess UE Assets", true)]
        [MenuItem("GameTools/Material/Preprocess UE Assets", true)]
        private static bool PreprocessUEAssetsValidation()
        {
            var modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject)) as ModelImporter;
            return modelImporter != null;
        }

        [MenuItem("Assets/Copy Material", false, 0)]
        [MenuItem("GameTools/Material/Copy")]
        private static void DoRecord()
        {
            foreach (Material m in Selection.GetFiltered(typeof(Material), SelectionMode.Assets))
            {
                mat = m;
            }
        }
        [MenuItem("Assets/Copy Material", true)]
        [MenuItem("GameTools/Material/Copy", true)]
        private static bool DoRecordValidation()
        {
            foreach (Material m in Selection.GetFiltered(typeof(Material), SelectionMode.Assets))
            {
                return true;
            }
            return false;
        }

        [MenuItem("Assets/Paste Material", false, 0)]
        [MenuItem("GameTools/Material/Paste")]
        private static void DoApply()
        {
            if (mat != null)
            {
                foreach (Material m in Selection.GetFiltered(typeof(Material), SelectionMode.Assets))
                {
                    Undo.RecordObject(m, "Paste Material");
                    m.shader = mat.shader;
                    m.CopyPropertiesFromMaterial(mat);
                    EditorUtility.SetDirty(m);
                }
            }
        }
        [MenuItem("Assets/Paste Material", true)]
        [MenuItem("GameTools/Material/Paste", true)]
        private static bool DoApplyValidation()
        {
            if (mat == null)
            {
                return false;
            }

            foreach (Material m in Selection.GetFiltered(typeof(Material), SelectionMode.Assets))
            {
                return true;
            }

            return false;
        }

        [MenuItem("Assets/Copy AN Textures", false, 0)]
        [MenuItem("GameTools/Material/Copy AN Textures")]
        private static void CopyAlbedoNormalTextures()
        {
            Material selectedMaterial = Selection.activeObject as Material;
            if (selectedMaterial != null)
            {
                albedoTexture = selectedMaterial.GetTexture("_MainTex") as Texture2D;
                normalTexture = selectedMaterial.GetTexture("_BumpMap") as Texture2D;
            }
        }
        [MenuItem("Assets/Copy AN Textures", true)]
        [MenuItem("GameTools/Material/Copy AN Textures", true)]
        private static bool CopyAlbedoNormalTexturesValidation()
        {
            return Selection.activeObject is Material;
        }

        [MenuItem("Assets/Paste AN Textures", false, 0)]
        [MenuItem("GameTools/Material/Paste AN Textures")]
        private static void PasteAlbedoNormalTextures()
        {
            Material selectedMaterial = Selection.activeObject as Material;
            if (selectedMaterial != null)
            {
                if (albedoTexture != null)
                {
                    selectedMaterial.SetTexture("_BaseMap", albedoTexture);
                    EditorUtility.SetDirty(selectedMaterial);
                }
                if (normalTexture != null)
                {
                    selectedMaterial.SetTexture("_BumpMap", normalTexture);
                    EditorUtility.SetDirty(selectedMaterial);
                }
            }
        }
        [MenuItem("Assets/Paste AN Textures", true)]
        [MenuItem("GameTools/Material/Paste AN Textures", true)]
        private static bool PasteAlbedoNormalTexturesValidation()
        {
            return Selection.activeObject is Material && (albedoTexture != null || normalTexture != null);
        }

        private static void ExtractMaterials(string assetPath, string destinationPath)
        {
            HashSet<string> hashSet = new HashSet<string>();
            IEnumerable<Object> enumerable = from x in AssetDatabase.LoadAllAssetsAtPath(assetPath)
                                             where x.GetType() == typeof(Material)
                                             select x;

            AssetImporter ai = AssetImporter.GetAtPath(assetPath);

            foreach (Object item in enumerable)
            {
                string path = System.IO.Path.Combine(destinationPath, item.name) + ".mat";
                if (File.Exists(path))
                {
                    ai.AddRemap(new AssetImporter.SourceAssetIdentifier(item), AssetDatabase.LoadMainAssetAtPath(path));
                    hashSet.Add(assetPath);
                }
                else
                {
                    path = AssetDatabase.GenerateUniqueAssetPath(path);
                    string value = AssetDatabase.ExtractAsset(item, path);
                    if (string.IsNullOrEmpty(value))
                    {
                        hashSet.Add(assetPath);
                    }
                }
            }

            ai.SaveAndReimport();

            foreach (string item2 in hashSet)
            {
                AssetDatabase.WriteImportSettingsIfDirty(item2);
                AssetDatabase.ImportAsset(item2, ImportAssetOptions.ForceUpdate);
            }
        }

        private static void AddToDeletedList(Transform root, List<Transform> deletedList)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                var child = root.transform.GetChild(i);
                if (child.gameObject.name.Contains("LOD0") || child.gameObject.name.Contains("LOD1") || child.gameObject.name.Contains("LOD2"))
                {
                    deletedList.Add(child);
                }
                AddToDeletedList(child, deletedList);
            }
        }
    }
}
#endif
