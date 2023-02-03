using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class SkyboxCapturer : MonoBehaviour
    {
        [SerializeField]
        [Min(1)]
        private int rt_scale = 4;

        private Camera _cam = null;
        private Camera cam
        {
            get
            {
                if (_cam == null)
                {
                    _cam = GetComponent<Camera>();
                }
                return _cam;
            }
        }

        private RenderTexture _rt = null;
        private RenderTexture rt
        {
            get
            {
                if (CameraManager.GetInstance() != null)
                {
                    var mainCam = CameraManager.GetInstance().GetMainCamera();
                    if (mainCam != null)
                    {
                        int w = mainCam.scaledPixelWidth / rt_scale;
                        int h = mainCam.scaledPixelHeight / rt_scale;
                        if (_rt == null)
                        {
                            // draw skybox twice is not needed
                            mainCam.clearFlags = CameraClearFlags.SolidColor;
                            _rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
                            _rt.name = gameObject.name;
                        }
                        if (_rt.width != w || _rt.height != h)
                        {
                            _rt.Release();
                            _rt.width = w;
                            _rt.height = h;
                            _rt.Create();
                            cam.ResetAspect();
                        }
                    }
                }
                return _rt;
            }
            set
            {
                _rt = value;
            }
        }

        public RenderTexture GetSnapshot()
        {
            if (Application.isPlaying)
            {
                return rt;
            }
            else
            {
                return null;
            }
        }

        private void Awake()
        {
            Utils.Assert(rt != null, gameObject.name + " RT is null.");

            if (cam != null)
            {
                cam.targetTexture = rt;
                cam.enabled = true;
            }
        }

        private void OnDestroy()
        {
            if (cam != null)
            {
                cam.targetTexture = null;
                cam.enabled = false;
            }
            if (rt != null)
            {
                Destroy(rt);
                rt = null;
            }
        }
    }
}
