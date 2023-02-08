using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameScript.UI.Common;
using TMPro;

namespace GameScript.UI.PaperUI
{
    public class PaperUI : UIBase
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
        private GameObject _paper3D = null;
        private GameObject paper3D
        {
            get
            {
                return _paper3D;
            }
        }

        [SerializeField]
        private Camera _paper3DCamera = null;
        private Camera paper3DCamera
        {
            get
            {
                return _paper3DCamera;
            }
        }

        [SerializeField]
        private TMP_Text _frontSideText = null;
        private TMP_Text frontSideText
        {
            get
            {
                return _frontSideText;
            }
        }

        [SerializeField]
        private TMP_Text _backsideText = null;
        private TMP_Text backsideText
        {
            get
            {
                return _backsideText;
            }
        }

        private bool isDoubleSide = false;
        private bool isMouseButtonDown = false;
        private Vector3 mousePosition = Vector3.zero;
        private Vector3 mouseButtonDownPaper3DEulerAngleBase = Vector3.zero;

        private Vector3 paper3DEulerAngleBase = new Vector3(-90, 0, 0);
        private Vector3 paper3DEulerAngleStart = new Vector3(10, 0, 10);
        private Vector3 paper3DEulerAngleEnd = new Vector3(-10, 0, -10);

        public void Show(bool isDoubleSide)
        {
            this.isDoubleSide = isDoubleSide;
            backsideText.gameObject.SetActive(false);
        }

        private void Start()
        {
            Show(true);
            closeButton.onClick.AddListener(CloseHandler);
        }

        private void Update()
        {
            UpdateDoubleSide();
            UpdatePaper3D();
        }

        private void CloseHandler()
        {
            UIManager.GetInstance().CloseUI(UIManager.UIName.Paper);
        }

        private void UpdateDoubleSide()
        {
            if (isDoubleSide)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isMouseButtonDown = true;
                    mousePosition = Input.mousePosition;
                }
                if (Input.GetMouseButtonUp(0) && isMouseButtonDown)
                {
                    isMouseButtonDown = false;
                    paper3DEulerAngleBase = mouseButtonDownPaper3DEulerAngleBase;
                }
                if (isMouseButtonDown)
                {
                    var delta = Input.mousePosition - mousePosition;
                    mouseButtonDownPaper3DEulerAngleBase = paper3DEulerAngleBase;
                    mouseButtonDownPaper3DEulerAngleBase.z -= delta.x * 0.35f;
                    paper3D.transform.localEulerAngles = mouseButtonDownPaper3DEulerAngleBase;
                }

                // text visible on double side
                {
                    var isFrontSide = Vector3.Dot(paper3D.transform.right, Vector3.right) > 0;
                    if (isFrontSide)
                    {
                        if (frontSideText.gameObject.activeSelf == false)
                        {
                            frontSideText.gameObject.SetActive(true);
                            backsideText.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (backsideText.gameObject.activeSelf == false)
                        {
                            backsideText.gameObject.SetActive(true);
                            frontSideText.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        private void UpdatePaper3D()
        {
            if (isMouseButtonDown == false)
            {
                var viewportPoint = paper3DCamera.ScreenToViewportPoint(Input.mousePosition);
                Vector3 paper3DEulerAngles = new Vector3(
                    Mathf.Lerp(paper3DEulerAngleBase.x + paper3DEulerAngleStart.x, paper3DEulerAngleBase.x + paper3DEulerAngleEnd.x, 1 - viewportPoint.y),
                    0,
                    Mathf.Lerp(paper3DEulerAngleBase.z + paper3DEulerAngleStart.z, paper3DEulerAngleBase.z + paper3DEulerAngleEnd.z, viewportPoint.x)
                );
                paper3D.transform.localEulerAngles = paper3DEulerAngles;
            }
        }
    }
}
