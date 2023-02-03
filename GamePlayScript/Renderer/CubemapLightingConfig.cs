using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.Cutscene;
using System;

namespace GameScript
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class CubemapLightingConfig : MonoBehaviour
    {
        [SerializeField]
        private GameObject _cubeBounds = null;
        public GameObject cubeBounds
        {
            get
            {
                return _cubeBounds;
            }
        }

        [SerializeField]
        private GameObject _pointLight = null;
        public GameObject pointLight
        {
            get
            {
                return _pointLight;
            }
        }

        [SerializeField]
        private Cubemap _cubemap = null;
        public Cubemap cubemap
        {
            get
            {
                return _cubemap;
            }
        }

        [SerializeField]
        private Camera _virtualCamera = null;
        public Camera virtualCamera
        {
            get
            {
                return _virtualCamera;
            }
        }

        [SerializeField]
        private GameObject _lookAtTarget = null;
        public GameObject lookAtTarget
        {
            get
            {
                return _lookAtTarget;
            }
        }

        [Range(0, 1)]
        [SerializeField]
        private float _shadowedRamp = 0.1f;
        public float shadowedRamp
        {
            get
            {
                return _shadowedRamp;
            }
        }

        [Range(0, 20)]
        [SerializeField]
        private float _lightingRange = 10;
        public float lightingRange
        {
            get
            {
                return _lightingRange;
            }
        }

        [SerializeField]
        private Vector3 _roleLightingOffset = Vector3.one;
        public Vector3 roleLightingOffset
        {
            get
            {
                return _roleLightingOffset;
            }
        }

        [SerializeField]
        private ViewField[] _viewFields = null;
        private ViewField[] viewFields
        {
            get
            {
                return _viewFields;
            }
        }
        public int NumberViewFields()
        {
            return viewFields == null ? 0 : viewFields.Length;
        }
        public ViewField GetViewField(int index)
        {
            if (viewFields == null || (index < 0 || index >= viewFields.Length))
            {
                return null;
            }
            return viewFields[index];
        }

        public bool IsEnabled()
        {
            return this.enabled;
        }

        private void Update()
        {
            if (virtualCamera != null && lookAtTarget != null)
            {
                virtualCamera.transform.LookAt(lookAtTarget.transform.position);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (pointLight != null)
            {
                Gizmos.DrawWireSphere(pointLight.transform.position, lightingRange);
            }
        }
#endif
    }
}
