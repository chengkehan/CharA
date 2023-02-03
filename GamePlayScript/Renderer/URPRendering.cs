using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

namespace GameScript
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class URPRendering : MonoBehaviour
    {
        public static URPRendering s_instance = null;

        public static URPRendering GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<URPRendering>();
            }
            return s_instance;
        }

        public ForwardRendererData forwardRendererData = null;

        private Dictionary<Type, ScriptableRendererFeature> rendererFeatures = new Dictionary<Type, ScriptableRendererFeature>();

        public T GetRendererFeatures<T>()
            where T : ScriptableRendererFeature
        {
            if (forwardRendererData == null)
            {
                return null;
            }

            if (rendererFeatures.TryGetValue(typeof(T), out ScriptableRendererFeature o))
            {
                return (T)o;
            }

            List<ScriptableRendererFeature> list = forwardRendererData.rendererFeatures;
            if (list == null || list.Count == 0)
            {
                return default(T);
            }

            Type tType = typeof(T);
            foreach (var item in list)
            {
                if (item.GetType() == tType)
                {
                    rendererFeatures.Add(tType, item);
                    return (T)item;
                }
            }

            return null;
        }
    }
}