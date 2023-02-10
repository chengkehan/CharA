using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameScript.UI.Common;
using TMPro;
using GameScript.UI.CentraPlan;
using GameScript.Cutscene;
using System;

namespace GameScript.UI.CardboardBoxUI
{
    public class CardboardBoxUI : UIBase
    {
        [SerializeField]
        private Button _closeButton = null;
        private Button closeButton
        {
            get
            {
                return _closeButton;
            }
        }

        [SerializeField]
        private GameObject _box3D = null;
        private GameObject box3D
        {
            get
            {
                return _box3D;
            }
        }

        [SerializeField]
        private HeroPanel _heroPanel = null;
        private HeroPanel heroPanel
        {
            get
            {
                return _heroPanel;
            }
        }

        [SerializeField]
        private Transform _spawnPoint = null;
        private Transform spawnPoint
        {
            get
            {
                return _spawnPoint;
            }
        }

        [SerializeField]
        private Transform _splashBoard = null;
        private Transform splashBoard
        {
            get
            {
                return _splashBoard;
            }
        }

        [SerializeField]
        private DropBoard _dropBoard = null;
        private DropBoard dropBoard
        {
            get
            {
                return _dropBoard;
            }
        }

        [SerializeField]
        private Camera _ui3dCamera = null;
        private Camera ui3dCamera
        {
            get
            {
                return _ui3dCamera;
            }
        }

        private MeshRenderer[] allMeshRenderers = null;

        private bool isMouseButtonDown = false;
        private Vector3 mousePosition = Vector3.zero;
        private Quaternion box3DRotationMouseDown = Quaternion.identity;

        private CardboardBox _cardboardBox = null;
        public CardboardBox cardboardBox
        {
            get
            {
                return _cardboardBox;
            }
            private set
            {
                _cardboardBox = value;
            }
        }

        private List<OneItem> allItems = null;

        public void Initialize(CardboardBox cardboardBox)
        {
            if (cardboardBox != null)
            {
                this.cardboardBox = cardboardBox;

                for (int i = 0; i < cardboardBox.pd.NumberItems(); i++)
                {
                    var itemPD = cardboardBox.pd.GetItem(i);
                    AddOneItem(itemPD);
                }

                StartCoroutine(RemoveSplashBoardDelayCoroutine());

                dropBoard.onTriggerEnter = DropBoardOnTriggerEnterHandler;
            }
        }

        private void AddOneItem(ItemPD itemPD)
        {
            AssetsManager.GetInstance().LoadSceneItem(itemPD.guid, itemPD.itemID, (go) =>
            {
                go.transform.SetParent(spawnPoint, false);
                go.transform.localPosition = Vector3.zero;
                Utils.SetLayerRecursively(go, (int)Define.Layers.UI3D);

                if (allItems == null)
                {
                    allItems = new List<OneItem>();
                }
                allItems.Add(new OneItem() { itemGO = go, itemGUID = itemPD.guid });

                OutlineObject.OnClick(go, () =>
                {
                    var notificationData = new TransferCardboardBoxItemToActorND();
                    notificationData.cardboardBoxGUID = cardboardBox.guid;
                    notificationData.itemGUID = itemPD.guid;
                    notificationData.actorGUID = ActorsManager.GetInstance().GetHeroActor().guid;
                    EventSystem.GetInstance().Notify(EventID.TransferCardboardBoxItemToActor, notificationData);
                });
            });
        }

        private void DropBoardOnTriggerEnterHandler(Collider collider)
        {
            if (cardboardBox != null && allItems != null && ActorsManager.GetInstance() != null)
            {
                var allItems = new List<OneItem>(this.allItems);
                foreach (var oneItem in allItems)
                {
                    if (oneItem != null && oneItem.itemGO != null && oneItem.itemGO.GetComponentInChildren<Collider>() == collider)
                    {
                        var heroActor = ActorsManager.GetInstance().GetHeroActor();
                        var notificationData = new TransferCardboardBoxItemToSceneND();
                        notificationData.cardboardBoxGUID = cardboardBox.guid;
                        notificationData.itemGUID = oneItem.itemGUID;
                        notificationData.dropPosition = DataCenter.query.AdjustSceneItemWorldPosition(heroActor.roleAnimation.GetMotionAnimator().GetPosition());
                        EventSystem.GetInstance().Notify(EventID.TransferCardboardBoxItemToScene, notificationData);
                    }
                }
            }
        }

        private IEnumerator RemoveSplashBoardDelayCoroutine()
        {
            yield return new WaitForSeconds(1f);
            splashBoard.gameObject.SetActive(false);
        }

        private void TransferPocketItemToCardboardBoxHandler(NotificationData _data)
        {
            var data = _data as TransferPocketItemToCardboardBoxND;
            if (data != null)
            {
                if (allItems != null && cardboardBox != null && cardboardBox.guid == data.cardboardBoxGUID)
                {
                    var cardboardBoxPD = DataCenter.GetInstance().playerData.GetSerializableMonoBehaviourPD<CardboardBoxPD>(data.cardboardBoxGUID);
                    for (int itemI = 0; itemI < cardboardBoxPD.NumberItems(); itemI++)
                    {
                        var itemPD = cardboardBoxPD.GetItem(itemI);
                        bool isExisted = false;
                        foreach (var oneItem in allItems)
                        {
                            if (oneItem != null && oneItem.itemGUID == itemPD.guid)
                            {
                                isExisted = true;
                                break;
                            }
                        }
                        if (isExisted == false)
                        {
                            AddOneItem(itemPD);
                        }
                    }
                }
            }
        }

        private void TransferCardboardBoxItemToActorHandler(NotificationData _data)
        {
            var data = _data as TransferCardboardBoxItemToActorND;
            if (data != null)
            {
                if (allItems != null && cardboardBox != null && cardboardBox.guid == data.cardboardBoxGUID)
                {
                    for (int itemI = 0; itemI < allItems.Count; itemI++)
                    {
                        var oneItem = allItems[itemI];
                        if (oneItem != null && oneItem.itemGUID == data.itemGUID)
                        {
                            allItems.RemoveAt(itemI);
                            AssetsManager.GetInstance().UnloadSceneItem(data.itemGUID);
                            break;
                        }
                    }
                }
            }
        }

        private void TransferCardboardBoxItemToSceneHandler(NotificationData _data)
        {
            var data = _data as TransferCardboardBoxItemToSceneND;
            if (data != null)
            {
                if (allItems != null && cardboardBox != null && cardboardBox.guid == data.cardboardBoxGUID && cardboardBox.pd.ContainsItem(data.itemGUID))
                {
                    AssetsManager.GetInstance().UnloadSceneItem(data.itemGUID);
                    for (int itemI = 0; itemI < allItems.Count; itemI++)
                    {
                        if (allItems[itemI].itemGUID == data.itemGUID)
                        {
                            allItems.RemoveAt(itemI);
                            break;
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (allItems != null)
            {
                for (int itemI = 0; itemI < allItems.Count; itemI++)
                {
                    if (allItems[itemI] != null)
                    {
                        AssetsManager.GetInstance().UnloadSceneItem(allItems[itemI].itemGUID);
                    }
                }
                allItems.Clear();
                allItems = null;
            }

            EventSystem.GetInstance().RemoveListener(EventID.TransferCardboardBoxItemToScene, TransferCardboardBoxItemToSceneHandler);
            EventSystem.GetInstance().RemoveListener(EventID.TransferCardboardBoxItemToActor, TransferCardboardBoxItemToActorHandler);
            EventSystem.GetInstance().RemoveListener(EventID.TransferPocketItemToCardboardBox, TransferPocketItemToCardboardBoxHandler);
        }

        private void Start()
        {
            closeButton.onClick.AddListener(CloseHandler);

            UpdateMaterials();

            EventSystem.GetInstance().AddListener(EventID.TransferCardboardBoxItemToScene, TransferCardboardBoxItemToSceneHandler);
            EventSystem.GetInstance().AddListener(EventID.TransferCardboardBoxItemToActor, TransferCardboardBoxItemToActorHandler);
            EventSystem.GetInstance().AddListener(EventID.TransferPocketItemToCardboardBox, TransferPocketItemToCardboardBoxHandler); 
        }

        private void CloseHandler()
        {
            UIManager.GetInstance().CloseUI(UIManager.UIName.CardboardBox);
        }

        private void Update()
        {
            UpdateMaterials();
            UpdateRotation();
            UpdateHeroPanel();
            Interactive3DDetector.DetectOutlineObject((int)Define.LayersMask.UI3D, ui3dCamera, true);
        }

        private void UpdateHeroPanel()
        {
            heroPanel.AlignToHero();
        }

        private void UpdateMaterials()
        {
            if (allMeshRenderers == null)
            {
                allMeshRenderers = box3D.GetComponentsInChildren<MeshRenderer>();
            }
            if (allMeshRenderers != null && DayNight.GetInstance() != null)
            {
                foreach (var meshRenderer in allMeshRenderers)
                {
                    if (meshRenderer != null)
                    {
                        var material = meshRenderer.material;
                        if (material != null)
                        {
                            // Keep 3d model lighting in ui
                            material.SetFloat(DayNight.GetInstance().dayNightProgressID, 1);
                        }
                    }
                }
            }
        }

        private void UpdateRotation()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isMouseButtonDown = true;
                mousePosition = Input.mousePosition;
                box3DRotationMouseDown = box3D.transform.rotation;
            }
            if (Input.GetMouseButtonUp(0) && isMouseButtonDown)
            {
                isMouseButtonDown = false;
            }
            if (isMouseButtonDown)
            {
                var delta = Input.mousePosition - mousePosition;
                box3D.transform.rotation = Quaternion.AngleAxis(-delta.x, Vector3.up) * Quaternion.AngleAxis(delta.y, Vector3.right) * box3DRotationMouseDown;
            }
        }

        private class OneItem
        {
            public GameObject itemGO = null;

            public string itemGUID = null;
        }
    }
}
