using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    [RequireComponent(typeof(Light))]
    public class GlobalEffectPointLight : MonoBehaviour
    {
        private static List<GlobalEffectPointLight> allPointLights = new List<GlobalEffectPointLight>();

        public static int NumberPointLights()
        {
            return allPointLights == null ? 0 : allPointLights.Count;
        }

        public static GlobalEffectPointLight GetPointLight(int index)
        {
            if (allPointLights == null || index < 0 || index >= allPointLights.Count)
            {
                return null;
            }
            else
            {
                return allPointLights[index];
            }
        }

        private Light pointLight = null;

        public bool active
        {
            get
            {
                return enabled &&
                        pointLight != null && pointLight.enabled &&
                        gameObject != null && gameObject.activeSelf && gameObject.activeInHierarchy;
            }
        }

        public Color color
        {
            get
            {
                return pointLight == null ? Color.white : pointLight.color;
            }
        }

        public Vector3 position
        {
            get
            {
                return pointLight == null && pointLight.transform == null ? Vector3.zero : pointLight.transform.position;
            }
        }

        public float range
        {
            get
            {
                return pointLight == null ? 1 : pointLight.range;
            }
        }

        public float intensity
        {
            get
            {
                return pointLight == null ? 0 : pointLight.intensity;
            }
        }

        private void Awake()
        {
            allPointLights.Add(this);
            pointLight = GetComponent<Light>();

            Utils.Assert(pointLight != null);
            Utils.Assert(pointLight.type == LightType.Point);
        }

        private void OnDestroy()
        {
            allPointLights.Remove(this);
        }
    }
}
