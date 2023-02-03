#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameScript.Cutscene;

[CustomEditor(typeof(ItemRefresh))]
public class ItemRefreshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("refresh item test"))
        {
            var itemRefresh = target as ItemRefresh;
            itemRefresh.RefreshItemNowTest();
        }
    }
}
#endif
