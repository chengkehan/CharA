using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameScript.UI.Common;
using TMPro;
using GameScript.UI.CentraPlan;

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

        private MeshRenderer[] allMeshRenderers = null;

        private bool isMouseButtonDown = false;
        private Vector3 mousePosition = Vector3.zero;
        private Quaternion box3DRotationMouseDown = Quaternion.identity;

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
    }
}
