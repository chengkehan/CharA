#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameScript;

namespace GameScriptEditor
{
    [CustomEditor(typeof(DayNight))]
    public class DayNightEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                var dayNight = target as DayNight;
                if (dayNight != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PrefixLabel("Progress");
                        dayNight.pd.dayNightProgress = EditorGUILayout.Slider(dayNight.pd.dayNightProgress, 0, 1);
                    }
                    EditorGUILayout.EndHorizontal(); 
                }
            }
        }
    }
}
#endif