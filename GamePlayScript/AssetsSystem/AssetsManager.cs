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
#if UNITY_EDITOR
        public const string EDITOR_CONFIG_PATH = "Assets/GameRes/Config/";

        public const string EDITOR_ROLE_HEAD_ICON_PATH = "Assets/GameRes/Editor/RoleHeadIcon/";
#endif

        public const string UI_ASSET_PREFIX = "UI_";

        public const string CONFIG_ASSET_PREFIX = "CFG_";

        public const string STORYBOARD_ASSET_PREFIX = "SB_";

        public const string ROLE_ASSET_PREFIX = "R_";

        public const string ITEM_MODEL_PREFIX = "IM_";

        public const string ITEM_ICON_PREFIX = "II_";

        public const string BUILDING_PREFAB_PREFIX = "BP_";

        public const string ROLE_CONFIG = CONFIG_ASSET_PREFIX + "Role";

        public const string LANGUAGE_CONFIG = CONFIG_ASSET_PREFIX + "Language";

        public const string ITEM_CONFIG = CONFIG_ASSET_PREFIX + "Item";

        public const string HERO_ROLE_ID = "200000";

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

        #region Item Icon

        private List<Sprite> _allSprite = new List<Sprite>();

        public void LoadItemIcon(string name, Action<Sprite> completeCB)
        {
            LoadAsset<Sprite>(ITEM_ICON_PREFIX + name, (obj)=>
            {
                _allSprite.Add(obj);
                completeCB?.Invoke(obj);
            });
        }

        // Don't release icon assets until loading a new scene.
        private void ReleaseAllSprite()
        {
            foreach (var sprite in _allSprite)
            {
                UnloadAsset(sprite);
            }
            _allSprite.Clear();
        }

        #endregion

        #region Scene Item

        private class SceneItemDelegate
        {
            public string guid = null;

            public GameObject go = null;
        }

        private Dictionary<string/*guid*/, SceneItemDelegate> _allSceneItem = new Dictionary<string, SceneItemDelegate>();

        public void LoadSceneItem(string guid, string name, Action<GameObject> completeCB)
        {
            Utils.Assert(ContainsSceneItem(guid) == false);

            var sceneItemDelegate = new SceneItemDelegate();
            sceneItemDelegate.guid = guid;
            _allSceneItem.Add(guid, sceneItemDelegate);

            LoadGameObject(ITEM_MODEL_PREFIX + name, (obj)=>
            {
                if (ContainsSceneItem(guid))
                {
                    sceneItemDelegate.go = obj;
                    completeCB?.Invoke(obj);
                }
                else
                {
                    UnloadGameObject(obj);
                }
            });
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

        public void UnloadGameObject(GameObject go)
        {
            if (go != null)
            {
                Addressables.ReleaseInstance(go);
            }
        }

        public void LoadAsset<T>(string name, Action<T> completeCB)
        {
            loadingCount++;

            var sceneName = SceneManager.GetInstance().sceneName;
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

        public void UnloadAsset<T>(T obj)
        {
            Addressables.Release(obj);
        }

        public void LoadScene(string name, Action completeCB)
        {
            ReleaseAllSprite();
            ReleaseAllSceneItemGo();

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