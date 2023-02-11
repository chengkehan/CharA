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

        private int boxCover1AnchorIndex = 0;
        [SerializeField]
        private Transform[] boxCover1Anchors = null;
        [SerializeField]
        private Transform _boxCover1 = null;
        private Transform boxCover1
        {
            get
            {
                return _boxCover1;
            }
        }

        private int boxCover2AnchorIndex = 0;
        [SerializeField]
        private Transform[] boxCover2Anchors = null;
        [SerializeField]
        private Transform _boxCover2 = null;
        private Transform boxCover2
        {
            get
            {
                return _boxCover2;
            }
        }

        private int boxCover3AnchorIndex = 0;
        [SerializeField]
        private Transform[] boxCover3Anchors = null;
        [SerializeField]
        private Transform _boxCover3 = null;
        private Transform boxCover3
        {
            get
            {
                return _boxCover3;
            }
        }

        private int boxCover4AnchorIndex = 0;
        [SerializeField]
        private Transform[] boxCover4Anchors = null;
        [SerializeField]
        private Transform _boxCover4 = null;
        private Transform boxCover4
        {
            get
            {
                return _boxCover4;
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

        private List<Transform> closedCovers = new List<Transform>();

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

        protected override void Awake()
        {
            base.Awake();

            OutlineObject.OnClick(boxCover1.gameObject, () =>
            {
                Utils.Log("Click cover1");
                if (closedCovers.Contains(boxCover1))
                {
                    closedCovers.Remove(boxCover1);
                    boxCover1AnchorIndex = 0;
                }
                else
                {
                    if (closedCovers.Count > 0 && closedCovers[closedCovers.Count - 1] == boxCover3)
                    {
                        boxCover1AnchorIndex = boxCover3AnchorIndex;
                    }
                    else
                    {
                        boxCover1AnchorIndex = boxCover1Anchors.Length - 1 - closedCovers.Count;
                    }
                    closedCovers.Add(boxCover1);
                }
            });
            OutlineObject.OnClick(boxCover2.gameObject, () =>
            {
                Utils.Log("Click cover2");
                if (closedCovers.Contains(boxCover2))
                {
                    closedCovers.Remove(boxCover2);
                    boxCover2AnchorIndex = 0;
                }
                else
                {
                    if (closedCovers.Count > 0 && closedCovers[closedCovers.Count - 1] == boxCover4)
                    {
                        boxCover2AnchorIndex = boxCover4AnchorIndex;
                    }
                    else
                    {
                        boxCover2AnchorIndex = boxCover2Anchors.Length - 1 - closedCovers.Count;
                    }
                    closedCovers.Add(boxCover2);
                }
            });
            OutlineObject.OnClick(boxCover3.gameObject, () =>
            {
                Utils.Log("Click cover3");
                if (closedCovers.Contains(boxCover3))
                {
                    closedCovers.Remove(boxCover3);
                    boxCover3AnchorIndex = 0;
                }
                else
                {
                    if (closedCovers.Count > 0 && closedCovers[closedCovers.Count - 1] == boxCover1)
                    {
                        boxCover3AnchorIndex = boxCover1AnchorIndex;
                    }
                    else
                    {
                        boxCover3AnchorIndex = boxCover3Anchors.Length - 1 - closedCovers.Count;
                    }
                    closedCovers.Add(boxCover3);
                }
            });
            OutlineObject.OnClick(boxCover4.gameObject, () =>
            {
                Utils.Log("Click cover4");
                if (closedCovers.Contains(boxCover4))
                {
                    closedCovers.Remove(boxCover4);
                    boxCover4AnchorIndex = 0;
                }
                else
                {
                    if (closedCovers.Count > 0 && closedCovers[closedCovers.Count - 1] == boxCover2)
                    {
                        boxCover4AnchorIndex = boxCover2AnchorIndex;
                    }
                    else
                    {
                        boxCover4AnchorIndex = boxCover4Anchors.Length - 1 - closedCovers.Count;
                    }
                    closedCovers.Add(boxCover4);
                }
            });
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
            UpdateCovers();
            Interactive3DDetector.DetectOutlineObject((int)Define.LayersMask.UI3D, ui3dCamera, true);
        }

        private void UpdateCovers()
        {
            float speed = 0.25f;

            boxCover1.rotation = Quaternion.Slerp(boxCover1.rotation, boxCover1Anchors[boxCover1AnchorIndex].rotation, speed);
            boxCover2.rotation = Quaternion.Slerp(boxCover2.rotation, boxCover2Anchors[boxCover2AnchorIndex].rotation, speed);
            boxCover3.rotation = Quaternion.Slerp(boxCover3.rotation, boxCover3Anchors[boxCover3AnchorIndex].rotation, speed);
            boxCover4.rotation = Quaternion.Slerp(boxCover4.rotation, boxCover4Anchors[boxCover4AnchorIndex].rotation, speed);

            OutlineObject.TurnOnOff(boxCover1.gameObject, closedCovers.Contains(boxCover1) == false || closedCovers.IndexOf(boxCover1) == closedCovers.Count - 1 || (closedCovers.IndexOf(boxCover1) == closedCovers.Count - 2 && closedCovers[closedCovers.Count - 1] == boxCover3));
            OutlineObject.TurnOnOff(boxCover2.gameObject, closedCovers.Contains(boxCover2) == false || closedCovers.IndexOf(boxCover2) == closedCovers.Count - 1 || (closedCovers.IndexOf(boxCover2) == closedCovers.Count - 2 && closedCovers[closedCovers.Count - 1] == boxCover4));
            OutlineObject.TurnOnOff(boxCover3.gameObject, closedCovers.Contains(boxCover3) == false || closedCovers.IndexOf(boxCover3) == closedCovers.Count - 1 || (closedCovers.IndexOf(boxCover3) == closedCovers.Count - 2 && closedCovers[closedCovers.Count - 1] == boxCover1));
            OutlineObject.TurnOnOff(boxCover4.gameObject, closedCovers.Contains(boxCover4) == false || closedCovers.IndexOf(boxCover4) == closedCovers.Count - 1 || (closedCovers.IndexOf(boxCover4) == closedCovers.Count - 2 && closedCovers[closedCovers.Count - 1] == boxCover2));
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
