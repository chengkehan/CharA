using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class LightShaftVolume : MonoBehaviour
    {
        private MeshRenderer _lightShaftMeshRenderer = null;
        private MeshRenderer lightShaftMeshRenderer
        {
            get
            {
                if (_lightShaftMeshRenderer == null)
                {
                    _lightShaftMeshRenderer = GetComponent<MeshRenderer>();
                    if (_lightShaftMeshRenderer == null)
                    {
                        _lightShaftMeshRenderer = GetComponentInChildren<MeshRenderer>();
                    }
                }
                return _lightShaftMeshRenderer;
            }
        }

        private Material _lightShaftMaterial = null;
        private Material lightShaftMaterial
        {
            get
            {
                if (_lightShaftMaterial == null)
                {
                    if (lightShaftMeshRenderer != null)
                    {
                        _lightShaftMaterial = lightShaftMeshRenderer.material;
                    }
                }
                return _lightShaftMaterial;
            }
        }

        private int _intensityPropID = 0;
        private int intensityPropID
        {
            get
            {
                if (_intensityPropID == 0)
                {
                    _intensityPropID = Shader.PropertyToID("_Intensity");
                }
                return _intensityPropID;
            }
        }

        [SerializeField]
        private BoundsComponent volumeBounds = new BoundsComponent();

        [SerializeField]
        private BoundsComponent outterVolumeBounds = new BoundsComponent();

        [SerializeField]
        private float speed = 5;

        [SerializeField]
        private float minIntensity = 0;

        private float maxIntensity = 0;

        private float _intensity = -1;
        private float intensity
        {
            get
            {
                return _intensity;
            }
            set
            {
                if (_intensity != value)
                {
                    _intensity = value;
                    if (lightShaftMaterial != null)
                    {
                        lightShaftMaterial.SetFloat(intensityPropID, _intensity);
                    }
                    if (lightShaftMeshRenderer != null)
                    {
                        lightShaftMeshRenderer.enabled = _intensity > 0;
                    }
                }
            }
        }

        private void Awake()
        {
            maxIntensity = 1;
            if (lightShaftMaterial != null)
            {
                maxIntensity = lightShaftMaterial.GetFloat(intensityPropID);
            }

            intensity = 0;
        }

        private void Update()
        {
            bool visible = false;
            Actor heroActor = null;
            if (ActorsManager.GetInstance() != null && ActorsManager.GetInstance().GetHeroActor() != null)
            {
                heroActor = ActorsManager.GetInstance().GetHeroActor();
            }
            if (heroActor != null && volumeBounds.InBounds(heroActor.roleAnimation.GetMotionAnimator().GetPosition()))
            {
                visible = true;
            }
            float speed = visible ? this.speed : -this.speed;
            speed *= Time.deltaTime;
            float minIntensity = 0;
            if (heroActor != null && outterVolumeBounds.InBounds(heroActor.roleAnimation.GetMotionAnimator().GetPosition()))
            {
                minIntensity = this.minIntensity;
            }
            intensity = Mathf.Clamp(intensity + speed, minIntensity, maxIntensity);
        }
    }
}
