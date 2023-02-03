#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using GameScript;

namespace GameScriptEditor
{
    [CustomPropertyDrawer(typeof(RenderingLayersMaskPropertyAttribute))]
    public class RenderingLayersMaskPropertyDrawer : PropertyDrawer
    {
        private static string[] m_DefaultRenderingLayerNames;
        internal static string[] defaultRenderingLayerNames
        {
            get
            {
                if (m_DefaultRenderingLayerNames == null)
                {
                    m_DefaultRenderingLayerNames = new string[31];
                    for (int i = 0; i < m_DefaultRenderingLayerNames.Length; ++i)
                    {
                        m_DefaultRenderingLayerNames[i] = string.Format("Layer{0}", i + 1);
                    }
                }
                return m_DefaultRenderingLayerNames;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {


            RenderPipelineAsset srpAsset = GraphicsSettings.currentRenderPipeline;
            bool usingSRP = srpAsset != null;
            if (!usingSRP) { return; }

            var layerNames = srpAsset.renderingLayerMaskNames;
            if (layerNames == null)
            {
                layerNames = defaultRenderingLayerNames;
            }


            object owner = GetParent(property);
            int mask = (int)this.fieldInfo.GetValue(owner);


            EditorGUI.BeginProperty(position, label, property);
            Rect fieldRect = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));
            int newMask = EditorGUI.MaskField(fieldRect, mask, layerNames);
            if (newMask != mask)
            {
                property.intValue = newMask;
            }

            EditorGUI.EndProperty();
        }

        private object GetParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        private object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        private object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }
    }
}
#endif