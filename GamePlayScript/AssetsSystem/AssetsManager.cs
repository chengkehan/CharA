using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using USM = UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameScript
{
    public class AssetsManager
    {
        public const string UI_ASSET_PREFIX = "UI_";

        public const string CONFIG_ASSET_PREFIX = "CFG_";

        public const string STORYBOARD_ASSET_PREFIX = "SB_";

        public const string ROLE_ASSET_PREFIX = "R_";

        public const string ITEM_MODEL_PREFIX = "IM_";

        public const string ITEM_ICON_PREFIX = "II_";

        public const string HEAD_ICON_PREFIX = "HI_";

        public const string BUILDING_PREFAB_PREFIX = "BP_";

        public const string ANIMATION_PREFIX = "ANIM_";

        public const string ROLE_CONFIG = CONFIG_ASSET_PREFIX + "Role";

        public const string LANGUAGE_CONFIG = CONFIG_ASSET_PREFIX + "Language";

        public const string ITEM_CONFIG = CONFIG_ASSET_PREFIX + "Item";

        private static AssetsManager s_instance = null;

        public static AssetsManager GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new AssetsManager();
            }
            return s_instance;
        }

        private Action _initializedCompleteCB = null;

        private bool _isInitialized = false;

        private SceneInstance currentSceneInstance;

        private int _loadingCount = 0;
        private int loadingCount
        {
            set
            {
                _loadingCount = value;
                // We can't do anything when assets is loading
                UIManager.GetInstance().eventSystemEnable = _loadingCount == 0;
            }
            get
            {
                return _loadingCount;
            }
        }

        public bool Initialize(Action completeCB)
        {
            if (GetIsInitialized())
            {
                return false;
            }
            SetIsInitialized(true);

            SetInitializedCompleteCB(completeCB);

            var addrInitAsync = Addressables.InitializeAsync();
            addrInitAsync.Completed += AddrInitAsyncCompleteHandler;

            return true;
        }

#if UNITY_EDITOR

        public static string GetEditorHeadIconPath(string roleId)
        {
            if (DataCenter.query.IsHeroRoleIdSimplified(roleId))
            {
                roleId = DataCenter.define.HeroRoleID;
            }
            return "Assets/GameRes/Role/" + roleId + "/" + roleId + ".png";
        }

#endif

        #region Animations

        private List<AnimationClip> _allAnimations = new List<AnimationClip>();

        public AnimationClip LoadAnimation(string animationAssetName)
        {
            var animationAsset = LoadAsset<AnimationClip>(ANIMATION_PREFIX + animationAssetName);
            _allAnimations.Add(animationAsset);
            return animationAsset;
        }

        private void ReleaseAllAnimations()
        {
            foreach (var animation in _allAnimations)
            {
                UnloadAsset(animation);
            }
            _allAnimations.Clear();
        }

        #endregion

        #region Head Icon

        private List<Sprite> _allHeadIcons = new List<Sprite>();

        public Sprite LoadHeadIcon(string roleId)
        {
            var icon = LoadAsset<Sprite>(HEAD_ICON_PREFIX + roleId);
            if (icon != null)
            {
                _allHeadIcons.Add(icon);
            }
            return icon;
        }

        private void ReleaseAllHeadIcons()
        {
            foreach (var sprite in _allHeadIcons)
            {
                UnloadAsset(sprite);
            }
            _allHeadIcons.Clear();
        }

        #endregion

        #region Item Icon

        private List<Sprite> _allItemIcons = new List<Sprite>();

        public Sprite LoadItemIcon(string name)
        {
            var icon = LoadAsset<Sprite>(ITEM_ICON_PREFIX + name);
            if (icon != null)
            {
                _allItemIcons.Add(icon);
            }
            return icon;
        }

        // Don't release icon assets until loading a new scene.
        private void ReleaseAllItemIcons()
        {
            foreach (var sprite in _allItemIcons)
            {
                UnloadAsset(sprite);
            }
            _allItemIcons.Clear();
        }

        #endregion

        #region Scene Item

        private class SceneItemDelegate
        {
            public string guid = null;

            public GameObject go = null;
        }

        private Dictionary<string/*guid*/, SceneItemDelegate> _allSceneItem = new Dictionary<string, SceneItemDelegate>();

        public GameObject LoadSceneItem(string itemGUID, string itemID)
        {
            Utils.Assert(ContainsSceneItem(itemGUID) == false);

            var sceneItemDelegate = new SceneItemDelegate();
            sceneItemDelegate.guid = itemGUID;
            _allSceneItem.Add(itemGUID, sceneItemDelegate);
            sceneItemDelegate.go = LoadGameObject(ITEM_MODEL_PREFIX + itemID);
            return sceneItemDelegate.go;
        }

        public void UnloadSceneItem(string guid)
        {
            if (ContainsSceneItem(guid))
            {
                var sceneItemDelegate = _allSceneItem[guid];
                UnloadGameObject(sceneItemDelegate.go);
                _allSceneItem.Remove(guid);
            }
        }

        private bool ContainsSceneItem(string guid)
        {
            return _allSceneItem.ContainsKey(guid);
        }

        private void ReleaseAllSceneItemGo()
        {
            foreach (var obj in _allSceneItem.Values)
            {
                UnloadGameObject(obj.go);
            }
            _allSceneItem.Clear();
        }

        #endregion

        #region Load GameObject

        public void LoadGameObject(string name, Action<GameObject> completeCB)
        {
            loadingCount++;

            Addressables.InstantiateAsync(name).Completed += (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Failed)
                {
                    Utils.LogError("Load error " + name);
                    Utils.Assert(false);
                }
                else
                {
                    completeCB?.Invoke(obj.Result);
                }

                loadingCount--;
            };
        }

        public GameObject LoadGameObject(string name)
        {
            var handle = Addressables.InstantiateAsync(name);
            var go = handle.WaitForCompletion();
            return go;
        }

        public void UnloadGameObject(GameObject go)
        {
            if (go != null)
            {
                Addressables.ReleaseInstance(go);
            }
        }

        #endregion

        #region Load Asset

        public void LoadAsset<T>(string name, Action<T> completeCB)
        {
            loadingCount++;

            Addressables.LoadAssetAsync<T>(name).Completed += (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Failed)
                {
                    Utils.LogError("Load error " + name);
                    Utils.Assert(false);
                }
                else
                {
                    completeCB?.Invoke(obj.Result);
                }

                loadingCount--;
            };
        }

        public T LoadAsset<T>(string name)
        {
            var handle = Addressables.LoadAssetAsync<T>(name);
            var asset = handle.WaitForCompletion();
            return asset;
        }

        public void UnloadAsset<T>(T obj)
        {
            if (obj != null)
            {
                Addressables.Release(obj);
            }
        }

        #endregion

        public void LoadScene(string name, Action completeCB)
        {
            ReleaseAllHeadIcons();
            ReleaseAllItemIcons();
            ReleaseAllSceneItemGo();
            ReleaseAllAnimations();

            Addressables.LoadSceneAsync(name, USM.LoadSceneMode.Single).Completed += (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Failed)
                {
                    Utils.LogError("Load error " + name);
                    Utils.Assert(false);
                }
                else
                {
                    USM.SceneManager.SetActiveScene(obj.Result.Scene);
                    completeCB?.Invoke();
                }
            };
        }

        public void UnloadScene()
        {
            Addressables.UnloadSceneAsync(currentSceneInstance);
        }

        public bool GetIsInitialized()
        {
            return _isInitialized;
        }

        private void SetIsInitialized(bool b)
        {
            _isInitialized = b;
        }

        private void SetInitializedCompleteCB(Action cb)
        {
            _initializedCompleteCB = cb;
        }

        private Action GetInitializedCompleteCB()
        {
            return _initializedCompleteCB;
        }

        private void AddrInitAsyncCompleteHandler(AsyncOperationHandle<IResourceLocator> obj)
        {
            if (obj.Status == AsyncOperationStatus.Failed)
            {
                Utils.Assert(false);
            }
            else
            {
                GetInitializedCompleteCB()?.Invoke();
            }
            SetInitializedCompleteCB(null);
            Addressables.Release(obj);
        }
    }
}