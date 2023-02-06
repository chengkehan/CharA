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

        private MeshRenderer[] allMeshRenderers = null;

        private bool isMouseButtonDown = false;
        private Vector3 mousePosition = Vector3.zero;
        private Quaternion box3DRotationMouseDown = Quaternion.identity;

        private CardboardBox cardboardBox = null;

        private List<OneItem> allItems = null;

        public void Initialize(CardboardBox cardboardBox)
        {
            if (cardboardBox != null)
            {
                this.cardboardBox = cardboardBox;

                for (int i = 0; i < cardboardBox.pd.NumberItems(); i++)
                {
                    var itemPD = cardboardBox.pd.GetItem(i);
                    var itemIndex = i;

                    AssetsManager.GetInstance().LoadSceneItem(itemPD.guid, itemPD.itemID, (go) =>
                    {
                        go.transform.SetParent(spawnPoint, false);
                        go.transform.localPosition = Vector3.zero;
                        Utils.SetLayerRecursively(go, (int)Define.Layers.UI3D);

                        if (allItems == null)
                        {
                            allItems = new List<OneItem>();
                        }
                        allItems.Add(new OneItem() { itemGO=go, itemIndex=itemIndex });
                    });
                }

                StartCoroutine(RemoveSplashBoardDelayCoroutine());

                dropBoard.onTriggerEnter = DropBoardOnTriggerEnterHandler;
            }
        }

        private void DropBoardOnTriggerEnterHandler(Collider collider)
        {
            if (cardboardBox != null && allItems != null)
            {
                foreach (var oneItem in allItems)
                {
                    if (oneItem != null && oneItem.itemGO != null && oneItem.itemGO.GetComponentInChildren<Collider>() == collider)
                    {
                        Utils.Log(cardboardBox.pd.GetItem(oneItem.itemIndex).itemID);
                        Utils.Log(cardboardBox.pd.GetItem(oneItem.itemIndex).guid);
                        break;
                    }
                }
            }
        }

        private IEnumerator RemoveSplashBoardDelayCoroutine()
        {
            yield return new WaitForSeconds(1f);
            splashBoard.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (cardboardBox != null)
            {
                for (int i = 0; i < cardboardBox.pd.NumberItems(); i++)
                {
                    var itemPD = cardboardBox.pd.GetItem(i);
                    AssetsManager.GetInstance().UnloadSceneItem(itemPD.guid);
                }
            }
        }

        private void Start()
        {
            closeButton.onClick.AddListener(CloseHandler);

            UpdateMaterials();
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

            public int itemIndex = 0;
        }
    }
}
