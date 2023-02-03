//=======================================================================
// Copyright (c) 2015 John Pan
// Distributed under the MIT License.
// (See accompanying file LICENSE or copy at
// http://opensource.org/licenses/MIT)
//=======================================================================

#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace SafeHandles
{
    [InitializeOnLoad]
    public static class HandlesHelper
    {
        static HandlesHelper()
        {
            SceneView.duringSceneGui += OnScene;
        }

        static readonly Dictionary<string,HandleInfo> HandleBuffers = new Dictionary<string, HandleInfo>(100);

        private static TInfo ActivateHandle<TInfo>(string id) where TInfo : HandleInfo, new()
        {
            HandleInfo info;
            if (!HandleBuffers.TryGetValue(id, out info))
            {
                info = new TInfo();
                HandleBuffers.Add(id, info);
            }
            info.Active = true;
            TInfo tInfo = info as TInfo;
            if (tInfo == null)
                throw new System.ArgumentException("Buffered handle is not a position handle!");
            return tInfo;
        }

        public static void DeleteHandle(string id)
        {
            HandleBuffers.Remove(id);
        }

        public static void WireCube(string id, Vector3 pos, Vector3 size, Color color)
        {
            var info = ActivateHandle<WireCubeHandleInfo>(id);
            info.position = pos;
            info.size = size;
            info.color = color;
            info.Update();
        }

        public static Vector3 PositionHandle(string id, Vector3 pos, Quaternion rot)
        {
            var posInfo = ActivateHandle<PositionHandleInfo>(id);
            if (posInfo.Changed == false)
            {
                posInfo.rotation = rot;
                posInfo._vector3Value = pos;
            } else
            {
                posInfo.Changed = false;
            }
            posInfo.Update();
            return posInfo.Vector3Value;
        }

        public static void LabelHandle(string id, string label, Vector3 pos, Color color)
        {
            var info = ActivateHandle<LabelHandleInfo>(id);
            info.position = pos;
            info.color = color;
            if (info.Changed == false)
            {
                info.label = new GUIContent(label);
            }
            info.Update();
        }

        static void OnScene(SceneView view)
        {
            foreach (HandleInfo info in HandleBuffers.Values)
            {
                if (info.Active)
                    info.Draw();
            }
        }

    #region Info types
        private abstract class HandleInfo
        {
            public bool Active;
            public int ID;

            public bool Changed { get; set; }

            public void Draw()
            {

                OnDraw();
            }

            public void Update()
            {
                Changed = false;
            }

            protected abstract void OnDraw();
        }

        private sealed class WireCubeHandleInfo : HandleInfo
        {
            public Vector3 position;

            public Vector3 size;

            public Color color;

            protected override void OnDraw()
            {
                Color handleColor = Handles.color;
                {
                    Handles.color = color;
                    Handles.DrawWireCube(position, size);
                }
                Handles.color = handleColor;
            }
        }

        private sealed class PositionHandleInfo : HandleInfo
        {
            public Quaternion rotation;
            public Vector3 _vector3Value;

            public Vector3 Vector3Value { get { return _vector3Value; } }

            protected override void OnDraw()
            {
                Vector3 lastValue = _vector3Value;
                _vector3Value = Handles.PositionHandle(_vector3Value, rotation);
                if (lastValue != _vector3Value)
                    Changed = true;
            }
        }

        private sealed class LabelHandleInfo : HandleInfo
        {
            public Vector3 position;
            public GUIContent label;
            public Color color;
            private GUIStyle style = new GUIStyle();

            protected override void OnDraw()
            {
                style.normal.textColor = color;
                Handles.Label(position, label, style);
            }
        }
    #endregion
    }
}
#endif
