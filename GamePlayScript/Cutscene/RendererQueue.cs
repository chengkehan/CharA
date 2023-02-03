using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class RendererQueue : MonoBehaviour
    {
        [SerializeField]
        private int queue = 2000;

        private void Awake()
        {
            RefreshQueue();
        }

        private void RefreshQueue()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                var material = meshRenderer.material;
                if (material != null)
                {
                    material.renderQueue = queue;
                }
            }
        }
    }
}
