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

        private Vector3[] boxMeshVertices = null;
        private Color[] boxMeshColors = null;
        private Mesh boxMesh = null;
        [SerializeField]
        private SkinnedMeshRenderer _boxRenderer = null;
        private SkinnedMeshRenderer boxRenderer
        {
            get
            {
                return _boxRenderer;
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
            var itemGo = AssetsManager.GetInstance().LoadSceneItem(itemPD.guid, itemPD.itemID);
            itemGo.transform.SetParent(spawnPoint, false);
            itemGo.transform.localPosition = Vector3.zero;
            Utils.SetLayerRecursively(itemGo, (int)Define.Layers.UI3D);

            if (allItems == null)
            {
                allItems = new List<OneItem>();
            }
            allItems.Add(new OneItem() { itemGO = itemGo, itemGUID = itemPD.guid });

            OutlineObject.OnClick(itemGo, () =>
            {
                ModeratorUtils.TransferCardboardBoxItemToActor(
                    cardboardBox.guid, itemPD.guid, ActorsManager.GetInstance().GetHeroActor().guid
                );
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
                        ModeratorUtils.TransferCardboardBoxItemToScene(
                            cardboardBox.guid, oneItem.itemGUID,
                            DataCenter.query.AdjustSceneItemWorldPosition(heroActor.roleAnimation.GetMotionAnimator().GetPosition())
                        );
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

            if (boxMesh != null)
            {
                Destroy(boxMesh);
                boxMesh = null;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            OutlineObject.OnClick(boxCover1.gameObject, () =>
            {
                if (closedCovers.Contains(boxCover1))
                {
                    closedCovers.Remove(boxCover1);
                    boxCover1AnchorIndex = 0;
                }
                else
                {
                    if (closedCovers.Count > 0 && closedCovers[closedCovers.Count - 1] == boxCover3)
                    {
                        boxCover1AnchorIndex = 2;
                    }
                    else
                    {
                        boxCover1AnchorIndex = 2;
                    }
                    closedCovers.Add(boxCover1);
                }
                RefreshCovers();
            });
            OutlineObject.OnClick(boxCover2.gameObject, () =>
            {
                if (closedCovers.Contains(boxCover2))
                {
                    closedCovers.Remove(boxCover2);
                    boxCover2AnchorIndex = 0;
                }
                else
                {
                    if (closedCovers.Count > 0 && closedCovers[closedCovers.Count - 1] == boxCover4)
                    {
                        boxCover2AnchorIndex = 2;
                    }
                    else
                    {
                        boxCover2AnchorIndex = 2;
                    }
                    closedCovers.Add(boxCover2);
                }
                RefreshCovers();
            });
            OutlineObject.OnClick(boxCover3.gameObject, () =>
            {
                if (closedCovers.Contains(boxCover3))
                {
                    closedCovers.Remove(boxCover3);
                    boxCover3AnchorIndex = 0;
                }
                else
                {
                    if (closedCovers.Count > 0 && closedCovers[closedCovers.Count - 1] == boxCover1)
                    {
                        boxCover3AnchorIndex = 2;
                    }
                    else
                    {
                        boxCover3AnchorIndex = 2;
                    }
                    closedCovers.Add(boxCover3);
                }
                RefreshCovers();
            });
            OutlineObject.OnClick(boxCover4.gameObject, () =>
            {
                if (closedCovers.Contains(boxCover4))
                {
                    closedCovers.Remove(boxCover4);
                    boxCover4AnchorIndex = 0;
                }
                else
                {
                    if (closedCovers.Count > 0 && closedCovers[closedCovers.Count - 1] == boxCover2)
                    {
                        boxCover4AnchorIndex = 2;
                    }
                    else
                    {
                        boxCover4AnchorIndex = 2;
                    }
                    closedCovers.Add(boxCover4);
                }
                RefreshCovers();
            });

            boxMesh = Instantiate(boxRenderer.sharedMesh);
            boxMesh.name = "CardboardDynamicMesh";
            boxMeshVertices = boxMesh.vertices;
            boxMeshColors = boxMesh.colors;
            boxRenderer.sharedMesh = boxMesh;
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

        private void RefreshCovers()
        {
            OutlineObject.TurnOnOff(boxCover1.gameObject, closedCovers.Contains(boxCover1) == false || closedCovers.IndexOf(boxCover1) == closedCovers.Count - 1 || (closedCovers.IndexOf(boxCover1) == closedCovers.Count - 2 && closedCovers[closedCovers.Count - 1] == boxCover3));
            OutlineObject.TurnOnOff(boxCover2.gameObject, closedCovers.Contains(boxCover2) == false || closedCovers.IndexOf(boxCover2) == closedCovers.Count - 1 || (closedCovers.IndexOf(boxCover2) == closedCovers.Count - 2 && closedCovers[closedCovers.Count - 1] == boxCover4));
            OutlineObject.TurnOnOff(boxCover3.gameObject, closedCovers.Contains(boxCover3) == false || closedCovers.IndexOf(boxCover3) == closedCovers.Count - 1 || (closedCovers.IndexOf(boxCover3) == closedCovers.Count - 2 && closedCovers[closedCovers.Count - 1] == boxCover1));
            OutlineObject.TurnOnOff(boxCover4.gameObject, closedCovers.Contains(boxCover4) == false || closedCovers.IndexOf(boxCover4) == closedCovers.Count - 1 || (closedCovers.IndexOf(boxCover4) == closedCovers.Count - 2 && closedCovers[closedCovers.Count - 1] == boxCover2));

            bool cover1Closed = closedCovers.Contains(boxCover1);
            int cover1Index = closedCovers.IndexOf(boxCover1);
            bool cover2Closed = closedCovers.Contains(boxCover2);
            int cover2Index = closedCovers.IndexOf(boxCover2);
            bool cover3Closed = closedCovers.Contains(boxCover3);
            int cover3Index = closedCovers.IndexOf(boxCover3);
            bool cover4Closed = closedCovers.Contains(boxCover4);
            int cover4Index = closedCovers.IndexOf(boxCover4);

            bool cover1LeftUp = cover1Closed && cover4Closed && cover1Index > cover4Index;
            bool cover1LeftDown = cover1Closed && cover4Closed && cover4Index > cover1Index;
            bool cover1RightUp = cover1Closed && cover2Closed && cover1Index > cover2Index;
            bool cover1RightDown = cover1Closed && cover2Closed && cover2Index > cover1Index;

            bool cover2LeftUp = cover2Closed && cover1Closed && cover2Index > cover1Index;
            bool cover2LeftDown = cover2Closed && cover1Closed && cover1Index > cover2Index;
            bool cover2RightUp = cover2Closed && cover3Closed && cover2Index > cover3Index;
            bool cover2RightDown = cover2Closed && cover3Closed && cover3Index > cover2Index;

            bool cover3LeftUp = cover3Closed && cover2Closed && cover3Index > cover2Index;
            bool cover3LeftDown = cover3Closed && cover2Closed && cover2Index > cover3Index;
            bool cover3RightUp = cover3Closed && cover4Closed && cover3Index > cover4Index;
            bool cover3RightDown = cover3Closed && cover4Closed && cover4Index > cover3Index;

            bool cover4LeftUp = cover4Closed && cover3Closed && cover4Index > cover3Index;
            bool cover4LeftDown = cover4Closed && cover3Closed && cover3Index > cover4Index;
            bool cover4RightUp = cover4Closed && cover1Closed && cover4Index > cover1Index;
            bool cover4RightDown = cover4Closed && cover1Closed && cover1Index > cover4Index;

            Vector3[] vertices = boxMesh.vertices;
            Color[] colors = boxMesh.colors;
            int numVertices = vertices.Length;
            float vertexOffset = 0.00025f;
            for (int i = 0; i < numVertices; i++)
            {
                Color c = colors[i];
                Vector3 v = boxMeshVertices[i];
                // Cover1
                {
                    if (c.r > 0.1f && c.r < 0.4f) // equals 0.3
                    {
                        if (cover1LeftUp)
                        {
                            v.y -= vertexOffset;
                        }
                        if (cover1LeftDown)
                        {
                            v.y += vertexOffset;
                        }
                    }
                    if (c.r > 0.4f && c.r < 0.7f) // equals 0.6
                    {
                        if (cover1RightUp)
                        {
                            v.y -= vertexOffset;
                        }
                        if (cover1RightDown)
                        {
                            v.y += vertexOffset;
                        }
                    }
                }
                // Cover2
                {
                    if (c.r > 0.7f && c.r < 1f) // equals 0.9
                    {
                        if (cover2LeftUp)
                        {
                            v.x -= vertexOffset;
                        }
                        if (cover2LeftDown)
                        {
                            v.x += vertexOffset;
                        }
                    }
                    if (c.g > 0.1f && c.g < 0.4f) // equals 0.3
                    {
                        if (cover2RightUp)
                        {
                            v.x -= vertexOffset;
                        }
                        if (cover2RightDown)
                        {
                            v.x += vertexOffset;
                        }
                    }
                }
                // Cover3
                {
                    if (c.g > 0.4f && c.g < 0.7f) // equals 0.6
                    {
                        if (cover3LeftUp)
                        {
                            v.y += vertexOffset;
                        }
                        if (cover3LeftDown)
                        {
                            v.y -= vertexOffset;
                        }
                    }
                    if (c.g > 0.7f && c.g < 1f) // equals 0.9
                    {
                        if (cover3RightUp)
                        {
                            v.y += vertexOffset;
                        }
                        if (cover3RightDown)
                        {
                            v.y -= vertexOffset;
                        }
                    }
                }
                // Cover4
                {
                    if (c.b > 0.1f && c.b < 0.4f) // equals 0.3
                    {
                        if (cover4LeftUp)
                        {
                            v.x += vertexOffset;
                        }
                        if (cover4LeftDown)
                        {
                            v.x -= vertexOffset;
                        }
                    }
                    if (c.b > 0.4f && c.b < 0.7f) // equals 0.6
                    {
                        if (cover4RightUp)
                        {
                            v.x += vertexOffset * (cover1RightUp ? 2 : 1); // a trick
                        }
                        if (cover4RightDown)
                        {
                            v.x -= vertexOffset;
                        }
                    }
                }

                vertices[i] = v;
            }
            boxMesh.vertices = vertices;
            boxMesh.colors = colors;
            boxMesh.UploadMeshData(false);
        }

        private void UpdateCovers()
        {
            float speed = 0.25f;

            boxCover1.rotation = Quaternion.Slerp(boxCover1.rotation, boxCover1Anchors[boxCover1AnchorIndex].rotation, speed);
            boxCover2.rotation = Quaternion.Slerp(boxCover2.rotation, boxCover2Anchors[boxCover2AnchorIndex].rotation, speed);
            boxCover3.rotation = Quaternion.Slerp(boxCover3.rotation, boxCover3Anchors[boxCover3AnchorIndex].rotation, speed);
            boxCover4.rotation = Quaternion.Slerp(boxCover4.rotation, boxCover4Anchors[boxCover4AnchorIndex].rotation, speed);
        }

        private void UpdateHeroPanel()
        {
            heroPanel.AlignToHero();
        }

        private void UpdateMaterials()
        {
            if (boxRenderer != null && boxRenderer.material != null)
            {
                // Keep 3d model lighting in ui
                boxRenderer.material.SetFloat(DayNight.GetInstance().dayNightProgressID, 1);
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
