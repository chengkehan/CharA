#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using StoryboardCore;
using GameScript;

namespace StoryboardEditor
{
    public class StoryNodeBaseEditor<T> : NodeEditorBase
        where T : StoryNodeBase
    {
        private List<NodePort> tempConnections = new List<NodePort>();

        private T _node = null;
        public T node
        {
            get
            {
                if (_node == null)
                {
                    _node = target as T;
                }
                return _node;
            }
        }

        protected Color GetTintByState(Color wannaColor)
        {
            if (node.state == StoryNodeState.Idle)
            {
                return wannaColor;
            }
            else if (node.state == StoryNodeState.Played)
            {
                return new Color(wannaColor.r * 0.8f, wannaColor.g * 0.8f, wannaColor.b * 0.8f);
            }
            else if (node.state == StoryNodeState.Playing)
            {
                return new Color(wannaColor.r * 1.3f, wannaColor.g * 1.3f, wannaColor.b * 1.3f);
            }
            else
            {
                return wannaColor;
            }
        }

        protected bool IsPortConnected(string portName)
        {
            foreach (var port in node.Ports)
            {
                if (port.fieldName == portName)
                {
                    return port.IsConnected;
                }
            }
            return false;
        }

        public override void OnValidate()
        {
            base.OnValidate();

            var nextPort = node.GetOutputPort(StoryNodeBase.NEXT_PORT_NAME);
            if (nextPort == null)
            {
                Validate(true,
                    true, string.Empty
                );
            }
            else
            {
                tempConnections.Clear();
                bool warning = false;
                string warningMsg = string.Empty;
                var connections = nextPort.GetConnections(tempConnections);
                if (connections != null && connections.Count > 1)
                {
                    var firstNextNode = connections[0].node;

                    // all connected nodes' type must be the same
                    var firstNextNodeType = firstNextNode.GetType();
                    for (int i = 1; i < connections.Count; i++)
                    {
                        if (connections[i].node.GetType() != firstNextNodeType)
                        {
                            warning = true;
                            warningMsg = "Connected nodes' types mismatched";
                            break;
                        }
                    }

                    // only ChoiceNode and LinkNode can be in parallel
                    if (warning == false)
                    {
                        if (!(firstNextNode is ChoiceNode) && !(firstNextNode is LinkNode))
                        {
                            warning = true;
                            warningMsg = "Connected nodes' can't be in parallel";
                        }
                    }
                }
                Validate(true,
                    !warning, warningMsg
                );
            }
        }
    }
}
#endif
