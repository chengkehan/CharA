#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNode;
using XNodeEditor;
using StoryboardCore;

namespace StoryboardEditor
{
    [CustomNodeGraphEditor(typeof(Storyboard))]
    public class StoryboardEditor : NodeGraphEditor
    {
        public override void OnOpen()
        {
            base.OnOpen();

            window.titleContent = new GUIContent("Storyboard");
        }

        public override string GetNodeMenuName(System.Type type)
        {
            if (type.Namespace == "StoryboardCore")
            {
                if (type.Name == "StoryNodeBase")
                {
                    return null;
                }
                else
                {
                    return base.GetNodeMenuName(type).Replace("StoryboardCore", "");
                }
            }
            else return null;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            OnGUI_Save();

            OnGUI_Warning();

            OnGUI_ThreadValue();

            OnGUI_LiveDebug();

            if (Application.isPlaying)
            {
                window.Repaint();
            }
        }

        private void OnGUI_Save()
        {
            if (GUI.Button(new Rect(0, 0, 80, 20), new GUIContent("Save", "Save manually")))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        #region Live Debug

        private List<StoryNodeBase> hightlightSelected = new List<StoryNodeBase>();

        private const string LIVE_DEBUG_GUI_KEY = "storyboard_liveDebugGUI";
        private bool liveDebug
        {
            get
            {
                return EditorPrefs.GetBool(LIVE_DEBUG_GUI_KEY, false);
            }
            set
            {
                EditorPrefs.SetBool(LIVE_DEBUG_GUI_KEY, value);
            }
        }

        private void OnGUI_LiveDebug()
        {
            var buttonRect = new Rect(20, window.position.height - 20, 20, 20);
            if (GUI.Button(buttonRect, new GUIContent("D", liveDebug ? "Live Debug On" : "Live Debug Off")))
            {
                liveDebug = !liveDebug;
            }

            if (Application.isPlaying && liveDebug)
            {
                var nodes = target.nodes;
                if (nodes != null)
                {
                    hightlightSelected.Clear();

                    foreach (var node in nodes)
                    {
                        if (node is StoryNodeBase)
                        {
                            var storyNode = node as StoryNodeBase;
                            if (storyNode.state == StoryNodeState.Playing)
                            {
                                hightlightSelected.Add(storyNode);
                            }
                        }
                    }

                    if (hightlightSelected.Count > 0)
                    {
                        Selection.objects = hightlightSelected.ToArray();
                        window.Home();
                    }
                }
            }
        }

        #endregion

        #region Thread Value

        private const string SHOW_THREAD_VALUE_GUI_KEY = "storyboard_showThreadValueGUI";
        private bool showThreadValues
        {
            get
            {
                return EditorPrefs.GetBool(SHOW_THREAD_VALUE_GUI_KEY, false);
            }
            set
            {
                EditorPrefs.SetBool(SHOW_THREAD_VALUE_GUI_KEY, value);
            }
        }

        private Vector2 threadValueScroll = Vector2.zero;

        private Dictionary<string/*valueName*/, List<ThreadValueNode>> allThreadValueNodes = new Dictionary<string, List<ThreadValueNode>>();

        private int jumpToThreadValueIndex = 0;

        private GUIContent threadValueGUIContent = new GUIContent();

        private void OnGUI_ThreadValue()
        {
            var buttonRect = new Rect(0, window.position.height - 20, 20, 20);
            if (GUI.Button(buttonRect, "V"))
            {
                showThreadValues = !showThreadValues;
            }
            if (showThreadValues)
            {
                CollectAllThreadValueNodes();

                // Draw
                float areaHeight = 200;
                float areaWidth = 200;
                var areaRect = new Rect(buttonRect.x, buttonRect.y - areaHeight, areaWidth, areaHeight);
                GUI.Box(areaRect, new GUIContent(string.Empty));
                GUILayout.BeginArea(areaRect);
                {
                    threadValueScroll = GUILayout.BeginScrollView(threadValueScroll);
                    {
                        foreach (var kv in allThreadValueNodes)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                // Persistent settings of nodes which are the same name should be matched.
                                bool isPersistentFieldTheSame = true;
                                {
                                    bool theFirstPersisteneField = kv.Value[0].persistent;
                                    for (int i = 1; i < kv.Value.Count; i++)
                                    {
                                        if (kv.Value[i].persistent != theFirstPersisteneField)
                                        {
                                            isPersistentFieldTheSame = false;
                                            break;
                                        }
                                    }
                                }

                                Color guiColor = GUI.color;
                                {
                                    if (isPersistentFieldTheSame == false)
                                    {
                                        GUI.color = Color.red;
                                    }
                                    string textValue = kv.Value[0].GetTextValue();
                                    threadValueGUIContent.text =
                                        kv.Key + " (" + kv.Value.Count + ") " +
                                        (string.IsNullOrWhiteSpace(textValue) ? "\"\"" : textValue) + "-" +
                                        kv.Value[0].GetNumberValue() + "-" +
                                        kv.Value[0].GetBooleanValue();
                                    if (isPersistentFieldTheSame == false)
                                    {
                                        threadValueGUIContent.tooltip = "persistent fields are not matched.\nbehavior is undefined.";
                                    }
                                    GUILayout.Label(threadValueGUIContent);
                                }
                                GUI.color = guiColor;

                                // Jump to next one
                                if (GUILayout.Button(">", GUILayout.Width(18)))
                                {
                                    ++jumpToThreadValueIndex;
                                    if (jumpToThreadValueIndex >= kv.Value.Count)
                                    {
                                        jumpToThreadValueIndex = 0;
                                    }
                                    Selection.objects = new Object[] { kv.Value[jumpToThreadValueIndex] };
                                    window.Home();
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndArea();
            }
        }

        private void CollectAllThreadValueNodes()
        {
            allThreadValueNodes.Clear();

            var nodes = target.nodes;
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (node is ThreadValueNode)
                    {
                        var threadValueNode = node as ThreadValueNode;
                        if (string.IsNullOrWhiteSpace(threadValueNode.valueName) == false)
                        {
                            if (allThreadValueNodes.TryGetValue(threadValueNode.valueName, out var threadValueNodeList) == false)
                            {
                                threadValueNodeList = new List<ThreadValueNode>();
                                allThreadValueNodes.Add(threadValueNode.valueName, threadValueNodeList);
                            }
                            threadValueNodeList.Add(threadValueNode);
                        }
                    }
                }
            }
        }

        #endregion

        #region OnGUI Warning

        private bool showWarning = false;

        private string warningText = string.Empty;

        private List<Node> warningNodes = new List<Node>();

        private void OnGUI_Warning()
        {
            showWarning = false;
            warningText = string.Empty;
            warningNodes.Clear();

            if (target != null && target.nodes != null)
            {
                ValidateWanringInformation(target.nodes);
            }
            if (showWarning)
            {
                GUI.DrawTexture(new Rect(0, 20, 50, 50), EditorTextures.storyboard_warning, ScaleMode.ScaleToFit);

                Color guiColor = GUI.color;
                {
                    GUI.color = new Color(0, 0, 0, 0);
                    if (GUI.Button(new Rect(0, 20, 50, 50), new GUIContent(string.Empty, warningText + "\nClick to jump to nodes.")))
                    {
                        Selection.objects = warningNodes.ToArray();
                        window.Home();
                    }
                }
                GUI.color = guiColor;
            }
        }

        private void ValidateWanringInformation(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is StoryNodeBase)
                {
                    var editor = NodeEditor.GetEditor(node, window) as NodeEditorBase;
                    if (editor.HasWarningInformation())
                    {
                        showWarning = true;
                        warningText = "Please resolve warning information of each node.";
                        warningNodes.Add(node);
                        return;
                    }
                }
            }
        }

        #endregion
    }
}
#endif
