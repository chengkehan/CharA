using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class CameraManager
    {
        private static CameraManager s_instance = null;

        public static CameraManager GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new CameraManager();
            }
            return s_instance;
        }

        private Camera _mainCamera = null;

        private Camera _uiCamera = null;

        public Camera GetMainCamera()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
            return _mainCamera;
        }

        public Vector3 GetMainCameraPosition()
        {
            Camera mainCam = GetMainCamera();
            if (mainCam == null || mainCam.transform == null)
            {
                return Vector3.zero;
            }
            else
            {
                return mainCam.transform.position;
            }
        }

        public Camera GetUICamera()
        {
            if (_uiCamera == null)
            {
                _uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            }
            return _uiCamera;
        }

        private SkyboxCapturer _skyboxCapturer = null;
        public SkyboxCapturer skyboxCapturer
        {
            get
            {
                if (_skyboxCapturer == null)
                {
                    var mainCam = GetMainCamera();
                    if (mainCam != null)
                    {
                        _skyboxCapturer = mainCam.GetComponentInChildren<SkyboxCapturer>();
                    }
                }
                return _skyboxCapturer;
            }
        }

        private SkyboxOcclusion _skyboxOcclusion = null;
        public SkyboxOcclusion skyboxOcclusion
        {
            get
            {
                if (_skyboxOcclusion == null)
                {
                    var mainCam = GetMainCamera();
                    if (mainCam != null)
                    {
                        _skyboxOcclusion = mainCam.GetComponentInChildren<SkyboxOcclusion>();
                    }
                }
                return _skyboxOcclusion;
            }
        }
    }
}
