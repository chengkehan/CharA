using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using GameScriptEditor;
using SafeHandles;
#endif

namespace GameScript.Cutscene
{
    [Serializable]
    public class BoundsComponent
    {
        [VisualizeBoundsAtrribute]
        [SerializeField]
        private Vector3 _boundsCenter = Vector3.zero;
        private Vector3 boundsCenter
        {
            get
            {
                return _boundsCenter;
            }
        }

        [SerializeField]
        private Vector3 _boundsSize = Vector3.one;
        private Vector3 boundsSize
        {
            get
            {
                return _boundsSize;
            }
        }

        [SerializeField]
        private Color _handlesColor = Color.white;

        // world space
        private Bounds _bounds = new Bounds();
        public Bounds bounds
        {
            get
            {
                _bounds.center = _boundsCenter;
                _bounds.size = _boundsSize;
                return _bounds;
            }
        }

        public bool InBounds(Vector3 pos)
        {
            return bounds.Contains(pos);
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class VisualizeBoundsAtrribute : PropertyAttribute
    {

    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(VisualizeBoundsAtrribute))]
    public class VisualizeBoundsAtrributeDrawer : PropertyDrawerWithEvent
    {
        private string handleID = null;

        private void DeleteHandles()
        {
            if (handleID != null)
            {
                HandlesHelper.DeleteHandle(handleID);
                HandlesHelper.DeleteHandle(handleID + "LLL");
                HandlesHelper.DeleteHandle(handleID + "CCC");
            }
        }

        public override void OnDestroy()
        {
            DeleteHandles();
        }

        public override void OnDisable()
        {
            DeleteHandles();
        }

        public override void OnEnable(Rect position, SerializedProperty property, GUIContent label)
        {
            // Do nothing
        }

        public override void OnGUIWithEvent(Rect position, SerializedProperty property, GUIContent label)
        {
            handleID = property.propertyPath;

            EditorGUI.PropertyField(position, property, label, true);

            property.vector3Value = HandlesHelper.PositionHandle(property.propertyPath, property.vector3Value, Quaternion.identity);

            var boundsSizeProp = property.serializedObject.FindProperty(property.propertyPath.Replace("_boundsCenter", "_boundsSize"));
            var handlesColorProp = property.serializedObject.FindProperty(property.propertyPath.Replace("_boundsCenter", "_handlesColor"));

            HandlesHelper.LabelHandle(property.propertyPath + "LLL", property.propertyPath, property.vector3Value, handlesColorProp.colorValue);
            HandlesHelper.WireCube(property.propertyPath + "CCC", property.vector3Value, boundsSizeProp.vector3Value, handlesColorProp.colorValue);

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }
#endif
}
