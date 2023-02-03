using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class DayNight : SerializableMonoBehaviour<DayNightPD>
    {
        private static DayNight s_instance = null;
        public static DayNight GetInstance()
        {
            return s_instance;
        }

        [SerializeField]
        private Light _directionalLight = null;
        private Light directionalLight
        {
            get
            {
                return _directionalLight;
            }
        }

        [SerializeField]
        private Transform _sunShape = null;
        private Transform sunShape
        {
            get
            {
                return _sunShape;
            }
        }

        private Material skyboxMaterial = null;

        private int _SunSizeID = 0;
        private int _SunSizeConvergenceID = 0;
        private int _AtmosphereThicknessID = 0;
        private int _ExposureID = 0;
        private int _lightDirOffsetID = 0;
        private int _DayNightProgressID = 0;
        private int _SunOcclusionID = 0;

        public int dayNightProgressID
        {
            get
            {
                return _DayNightProgressID;
            }
        }

        public float sunSize
        {
            set
            {
                SetSkyboxMaterialFloat(_SunSizeID, value);
            }
            get
            {
                return GetSkyboxMaterialFloat(_SunSizeID);
            }
        }

        public float sunSizeConvergence
        {
            set
            {
                SetSkyboxMaterialFloat(_SunSizeConvergenceID, value);
            }
            get
            {
                return GetSkyboxMaterialFloat(_SunSizeConvergenceID);
            }
        }

        public float atmosphereThickness
        {
            set
            {
                SetSkyboxMaterialFloat(_AtmosphereThicknessID, value);
            }
            get
            {
                return GetSkyboxMaterialFloat(_AtmosphereThicknessID);
            }
        }

        public float exposure
        {
            set
            {
                SetSkyboxMaterialFloat(_ExposureID, value);
            }
            get
            {
                return GetSkyboxMaterialFloat(_ExposureID);
            }
        }

        public Vector3 lightDirOffset
        {
            set
            {
                SetSkyboxMaterialVector(_lightDirOffsetID, value);
            }
            get
            {
                return GetSkyboxMaterialVector(_lightDirOffsetID);
            }
        }

        public Vector3 lightEulerAngles
        {
            set
            {
                if (directionalLight != null && directionalLight.transform != null)
                {
                    directionalLight.transform.eulerAngles = value;
                }
            }
            get
            {
                return directionalLight == null || directionalLight.transform == null ? Vector3.zero : directionalLight.transform.eulerAngles;
            }
        }

        public Color lightColor
        {
            set
            {
                if (directionalLight != null)
                {
                    directionalLight.color = value;
                }
            }
            get
            {
                return directionalLight == null ? Color.white : directionalLight.color;
            }
        }

        public Vector3 sunPosition
        {
            get
            {
                if (directionalLight != null && directionalLight.transform != null)
                {
                    Vector3 lightDir = -directionalLight.transform.forward;
                    lightDir += lightDirOffset;
                    lightDir.Normalize();
                    Vector3 sunPosition = lightDir * 2999;
                    return sunPosition;
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }

        private float sunShapeSize
        {
            get
            {
                return 80;
            }
        }

        private void SetSkyboxMaterialFloat(int id, float value)
        {
            if (skyboxMaterial != null)
            {
                skyboxMaterial.SetFloat(id, value);
            }
        }

        private float GetSkyboxMaterialFloat(int id)
        {
            if (skyboxMaterial != null)
            {
                return skyboxMaterial.GetFloat(id);
            }
            else
            {
                return 0;
            }
        }

        private void SetSkyboxMaterialVector(int id, Vector3 value)
        {
            if (Application.isPlaying && skyboxMaterial != null)
            {
                skyboxMaterial.SetVector(id, value);
            }
        }

        private Vector3 GetSkyboxMaterialVector(int id)
        {
            if (skyboxMaterial != null)
            {
                return skyboxMaterial.GetVector(id);
            }
            else
            {
                return Vector4.zero;
            }
        }

        protected override void InitializeOnAwake()
        {
            base.InitializeOnAwake();

            if (Application.isPlaying)
            {
                Utils.Assert(RenderSettings.skybox != null);
                skyboxMaterial = new Material(RenderSettings.skybox);
                RenderSettings.skybox = skyboxMaterial;
            }
            else
            {
                skyboxMaterial = RenderSettings.skybox;
            }

            _SunSizeID = Shader.PropertyToID("_SunSize");
            _SunSizeConvergenceID = Shader.PropertyToID("_SunSizeConvergence");
            _AtmosphereThicknessID = Shader.PropertyToID("_AtmosphereThickness");
            _ExposureID = Shader.PropertyToID("_Exposure");
            _lightDirOffsetID = Shader.PropertyToID("_lightDirOffset");
            _DayNightProgressID = Shader.PropertyToID("_DayNightProgress");
            _SunOcclusionID = Shader.PropertyToID("_SunOcclusion");

            s_instance = this;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            s_instance = null;
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                Shader.SetGlobalFloat(_DayNightProgressID, Mathf.GammaToLinearSpace(pd.dayNightProgress));
                lightEulerAngles = Vector3.Lerp(new Vector3(23.40527f, 307.3426f, 323.8654f), new Vector3(55.43285f, 348.8549f, 23.54967f), pd.dayNightProgress);
               // sunSize = Mathf.Lerp(0f, 0.021f, pd.dayNightProgress);
                lightColor = Color.Lerp(new Color(1, 1, 1), new Color(1f, 0.9568627f, 0.8392157f), pd.dayNightProgress);

                sunShape.position = sunPosition;
                sunShape.localScale = Vector3.one * sunShapeSize * 2;

                UpdateSunShapeRaycastHit();
            }
            else
            {
                Shader.SetGlobalFloat(_DayNightProgressID, 1);
                Shader.SetGlobalFloat(_SunOcclusionID, 0);
            }
        }

        #region Sun Shape Raycast Hit

        private float _sunOcclusion = 0;
        private float _sunOcclusionTarget = 0;

        private void UpdateSunShapeRaycastHit()
        {
            if (CameraManager.GetInstance() != null)
            {
                var sunPos = sunPosition;
                var mainCamPos = CameraManager.GetInstance().GetMainCameraPosition();

                if (allSunShapeRays == null)
                {
                    allSunShapeRays = new Ray[17];
                }
                allSunShapeRays[0] = new Ray(mainCamPos, sunPos - mainCamPos);
                int rayIndex = 0;
                for (float radius = 0.45f; radius < 1; radius+=0.45f)
                {
                    FillSunShapeRays(++rayIndex, ++rayIndex, radius, Vector3.up, sunPos, mainCamPos);
                    FillSunShapeRays(++rayIndex, ++rayIndex, radius, Vector3.right, sunPos, mainCamPos);
                    FillSunShapeRays(++rayIndex, ++rayIndex, radius, (Vector3.right + Vector3.up).normalized, sunPos, mainCamPos);
                    FillSunShapeRays(++rayIndex, ++rayIndex, radius, (Vector3.left + Vector3.up).normalized, sunPos, mainCamPos);
                }

                int numHitRays = 0;
                foreach (var ray in allSunShapeRays)
                {
                    int numHits = Physics.RaycastNonAlloc(ray, GetSunShapeHitInfoBuffer(), 3999, GetLayerMaskOfSunShape());
                    if (numHits > 0)
                    {
                        System.Array.Sort(GetSunShapeHitInfoBuffer(), 0, numHits, GetHitInfoSortingComparer());
                        var firstHit = GetSunShapeHitInfoBuffer()[0];
                        if (firstHit.collider.transform == sunShape)
                        {
                            ++numHitRays;
                        }
                    }
                }
                float hitRate = (float)numHitRays / (float)allSunShapeRays.Length;
                _sunOcclusionTarget = Mathf.Clamp01(1 - hitRate);
                _sunOcclusion = Mathf.Clamp01(_sunOcclusion + (_sunOcclusionTarget - _sunOcclusion) * 0.5f);
                Shader.SetGlobalFloat(_SunOcclusionID, _sunOcclusion);
            }
        }

        private void FillSunShapeRays(int index1, int index2, float radius, Vector3 refDir, Vector3 sunPos, Vector3 mainCamPos)
        {
            var dir = sunPos - mainCamPos;
            var offsetDir = Vector3.Cross(dir, refDir);
            offsetDir.Normalize();
            {
                var tSunPos = sunPos + offsetDir * sunShapeSize * radius;
                allSunShapeRays[index1] = new Ray(mainCamPos, tSunPos - mainCamPos);
            }
            {
                var tSunPos = sunPos + offsetDir * sunShapeSize * -radius;
                allSunShapeRays[index2] = new Ray(mainCamPos, tSunPos - mainCamPos);
            }
        }

        private Ray[] allSunShapeRays = null;
        private Interactive3DDetector.HitInfoSortingComparer _hitInfoSortingComparer = null;
        private RaycastHit[] _sunShapeHitInfoBuffer = null;
        private int _layerMaskOfSunShape = 0;
        private int GetLayerMaskOfSunShape()
        {
            if (_layerMaskOfSunShape == 0)
            {
                _layerMaskOfSunShape = LayerMask.GetMask(new string[] { "SunShape" });
            }
            return _layerMaskOfSunShape;
        }
        private RaycastHit[] GetSunShapeHitInfoBuffer()
        {
            if (_sunShapeHitInfoBuffer == null)
            {
                _sunShapeHitInfoBuffer = new RaycastHit[5];
            }
            return _sunShapeHitInfoBuffer;
        }
        private Interactive3DDetector.HitInfoSortingComparer GetHitInfoSortingComparer()
        {
            if (_hitInfoSortingComparer == null)
            {
                _hitInfoSortingComparer = new Interactive3DDetector.HitInfoSortingComparer();
            }
            return _hitInfoSortingComparer;
        }

        private void OnDrawGizmo_SunShapeRaycastHit()
        {
            if (Application.isPlaying)
            {
                if (allSunShapeRays != null)
                {
                    Gizmos.color = Color.yellow;
                    Vector3 sunPosition = this.sunPosition;
                    foreach (var ray in allSunShapeRays)
                    {
                        Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * Vector3.Magnitude(sunPosition - ray.origin));
                    }
                }
            }
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (directionalLight != null && directionalLight.transform != null &&
                CameraManager.GetInstance() != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.matrix = Matrix4x4.identity;

                Vector3 sunPosition = this.sunPosition;
                Gizmos.DrawSphere(sunPosition, sunShapeSize);
                //Gizmos.DrawLine(CameraManager.GetInstance().GetMainCameraPosition(), sunPosition);

                OnDrawGizmo_SunShapeRaycastHit();
            }
        }
#endif
    }
}
