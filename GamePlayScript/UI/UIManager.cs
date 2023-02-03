using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UEvt = UnityEngine.EventSystems;

namespace GameScript
{
    public class UIManager
    {
        public enum UIName
        {
            Talking,
            HUD,
            MenuBar,
            CentraPlan,
            Paper,
            CardboardBox
        }

        public static UIManager s_instance = null;

        private const int DEPTH_STEP = 50;

        public static UIManager GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new UIManager();
            }
            return s_instance;
        }

        #region UI Depth

        public enum Depth
        {
            Normal = 0,
            Overlay = 100,
            Modal = 200
        }

        private Dictionary<UIName, Depth> _uisDepth = new Dictionary<UIName, Depth>() {
            //{ UIName.CentraPlan, Depth.Overlay }
        };
        private Depth GetUIDepth(UIName uiName)
        {
            if (_uisDepth.TryGetValue(uiName, out Depth depth))
            {
                return depth;
            }
            else
            {
                return Depth.Normal;
            }
        }

        private void RefreshAllUIDepth()
        {
            for (int i = 0; i < allUIInstances.Count; i++)
            {
                var uiInstance = allUIInstances[i];
                var depth = GetUIDepth(uiInstance.name);
                int sameDepthCount = 0;
                for (int j = 0; j < allUIInstances.Count; j++)
                {
                    var anotherUIInstance = allUIInstances[j];
                    if (uiInstance != anotherUIInstance && depth == GetUIDepth(anotherUIInstance.name))
                    {
                        ++sameDepthCount;
                    }
                }
                if (uiInstance.IsAssetReady())
                {
                    uiInstance.canvas.sortingOrder = (int)depth + sameDepthCount;
                }
            }
        }

        #endregion

        #region UI Opposite

        private Dictionary<UIName, UIName[]> _uiOpposite = new Dictionary<UIName, UIName[]>() {
            { UIName.CentraPlan, new UIName[]{ UIName.HUD } },
            { UIName.Paper, new UIName[]{ UIName.HUD } },
            { UIName.CardboardBox, new UIName[]{ UIName.HUD } }
        };

        private void RefreshAllUIOpposite()
        {
            for (int i = 0; i < allUIInstances.Count; i++)
            {
                var uiInstance = allUIInstances[i];
                bool isVisible = true;

                foreach (var kv in _uiOpposite)
                {
                    if (Array.IndexOf(kv.Value, uiInstance.name) != -1)
                    {
                        for (int j = 0; j < allUIInstances.Count; j++)
                        {
                            var anotherUIInstance = allUIInstances[j];
                            if (anotherUIInstance.name == kv.Key)
                            {
                                isVisible = false;
                            }
                        }
                    }
                }

                if (uiInstance.IsAssetReady())
                {
                    uiInstance.gameObject.SetActive(isVisible);
                }
            }
        }

        #endregion

        private Transform _uiRoot = null;

        private List<UIInstance> allUIInstances = new List<UIInstance>();

        public bool OpenUI(UIName uiName, Action completeCB)
        {
            if (ContainsUI(uiName))
            {
                return false;
            }
            else
            {
                UIInstance uiInstance = new UIInstance();
                uiInstance.name = uiName;
                allUIInstances.Add(uiInstance);
                AssetsManager.GetInstance().LoadAsset<UnityEngine.Object>(AssetsManager.UI_ASSET_PREFIX + uiName.ToString(), (obj)=>
                {
                    uiInstance.prefab = obj;
                    GameObject ui = Utils.InstantiateUIPrefab(uiInstance.prefab, GetUIRoot());
                    uiInstance.gameObject = ui;
                    uiInstance.canvas = ui.GetComponent<Canvas>();
                    Utils.Assert(uiInstance.canvas != null, "UI Canvas is missing");
                    uiInstance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    uiInstance.canvas.worldCamera = CameraManager.GetInstance().GetUICamera();
                    RefreshAllUIDepth();
                    RefreshAllUIOpposite();

                    completeCB?.Invoke();
                });
                return true;
            }
        }

        public bool CloseUI(UIName uiName)
        {
            if (ContainsUI(uiName) == false)
            {
                return false;
            }
            else
            {
                UIInstance uiInstance = GetUI(uiName);
                allUIInstances.Remove(uiInstance);
                Utils.Destroy(uiInstance.gameObject);
                AssetsManager.GetInstance().UnloadAsset(uiInstance.prefab);
                RefreshAllUIDepth();
                RefreshAllUIOpposite();
                return true;
            }
        }

        public UIT GetUI<UIT>(UIName uiName) 
            where UIT : MonoBehaviour
        {
            if (ContainsUI(uiName))
            {
                var uiInstance = GetUI(uiName);
                if (uiInstance.gameObject == null)
                {
                    return default(UIT);
                }
                else
                {
                    var ui = uiInstance.gameObject.GetComponent<UIT>();
                    return ui;
                }
            }
            else
            {
                return default(UIT);
            }
        }

        public bool ContainsUI(UIName uiName)
        {
            foreach (var uiInstance in allUIInstances)
            {
                if (uiInstance.name == uiName)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsUIBreakInteractiveScene()
        {
            return ContainsUI(UIName.Talking);
        }

        #region UGUI EventSystem

        private UEvt.EventSystem _eventSystem = null;
        private UEvt.EventSystem eventSystem
        {
            get
            {
                return _eventSystem;
            }
        }

        public bool eventSystemEnable
        {
            set
            {
                if (value != eventSystem.enabled)
                {
                    eventSystem.enabled = value;

                    var notificationData = new UGUI_EventSystemChangedND();
                    notificationData.enabled = value;
                    EventSystem.GetInstance().Notify(EventID.UGUI_EventSystemChanged, notificationData);
                }
            }
            get
            {
                return eventSystem == null  ? false : eventSystem.enabled;
            }
        }

        private void InitEventSystem()
        {
            _eventSystem = UEvt.EventSystem.current;
        }

        #endregion

        public bool IsMouseOverUI()
        {
            if (eventSystemEnable == false)
            {
                return false;
            }

#if IPHONE || ANDROID
            if (eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
            if (eventSystem.IsPointerOverGameObject())
#endif
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Initialize()
        {
            // Do nothing
        }

        private UIManager()
        {
            InitEventSystem();
        }

        private UIInstance GetUI(UIName uiName)
        {
            foreach (var uiInstance in allUIInstances)
            {
                if (uiInstance.name == uiName)
                {
                    return uiInstance;
                }
            }
            return null;
        }

        private Transform GetUIRoot()
        {
            if (_uiRoot == null)
            {
                _uiRoot = GameObject.Find("UIRoot").transform;
            }
            return _uiRoot;
        }

        private class UIInstance
        {
            public UnityEngine.Object prefab = null;

            public UIName name;

            public GameObject gameObject = null;

            public Canvas canvas = null;

            public bool IsAssetReady()
            {
                return canvas != null;
            }
        }
    }
}
