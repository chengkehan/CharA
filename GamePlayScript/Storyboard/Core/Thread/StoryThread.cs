using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript;
using XNode;
using System;

namespace StoryboardCore
{
    public class StoryThread
    {
        public class AsyncHandler<T>
            where T : StoryNodeBase
        {
            private StoryThread storyThread = null;

            private List<T> _nodes = null;
            public List<T> nodes
            {
                private set
                {
                    _nodes = value;
                }
                get
                {
                    return _nodes;
                }
            }

            public AsyncHandler(StoryThread storyThread, List<T> nodes)
            {
                this.storyThread = storyThread;
                this.nodes = nodes;
            }

            public void Complete(T selectedNode = null)
            {
                if (selectedNode == null)
                {
                    selectedNode = nodes[0];
                }

                Utils.Assert(nodes.IndexOf(selectedNode) != -1, "Invalid selected node.");

                if (selectedNode is EndNode)
                {
                    storyThread.CleanupWhenComplete();
                }
                else
                {
                    storyThread.ClearCurrentNodes(StoryNodeState.Idle);
                    storyThread.AddCurrentNode(selectedNode);
                    storyThread.FetchNextNodes();
                    storyThread.Play();
                }
            }
        }

        public delegate void AsyncCallback<T>(AsyncHandler<T> asyncHandler) where T : StoryNodeBase;

        private Storyboard storyboard = null;

        #region currentNodes

        private List<StoryNodeBase> _currentNodes = new List<StoryNodeBase>();

        private int NumberCurrentNodes()
        {
            return _currentNodes.Count;
        }

        private StoryNodeBase GetCurrentNode(int index)
        {
            return _currentNodes[index];
        }

        private void ClearCurrentNodes(StoryNodeState setAsThisState)
        {
            for (int i = 0; i < NumberCurrentNodes(); i++)
            {
                GetCurrentNode(i).state = setAsThisState;
            }
            _currentNodes.Clear();
        }

        private void AddCurrentNode(StoryNodeBase node)
        {
            _currentNodes.Add(node);
            node.state = StoryNodeState.Playing;
        }

        #endregion

        private List<ITriggerableNode> currentTriggerableNodes = new List<ITriggerableNode>();

        private AsyncCallback<TalkNode> talkAsyncCallback = null;

        private AsyncCallback<ChoiceNode> choiceAsyncCallback = null;

        private AsyncCallback<EndNode> endAsyncCallback = null;

        private StoryThreadPD _localStoryThreadPD = null;
        public StoryThreadPD localStoryThreadPD
        {
            private set
            {
                _localStoryThreadPD = value;
            }
            get
            {
                return _localStoryThreadPD;
            }
        }

        public StoryThreadPD persistentStoryThreadPD
        {
            get
            {
                return DataCenter.GetInstance().playerData.GetStoryThreadPD(storyboard.guid);
            }
        }

        public StoryThread(
            Storyboard storyboard,
            AsyncCallback<TalkNode> talkAsyncCallback, AsyncCallback<ChoiceNode> choiceAsyncCallback,
            AsyncCallback<EndNode> endAsyncCallback)
        {
            Utils.Assert(storyboard != null, "Storyboard is null");
            Utils.Assert(talkAsyncCallback != null, "talkAsyncCallback is null");
            Utils.Assert(choiceAsyncCallback != null, "choiceAsyncCallback is null");
            Utils.Assert(endAsyncCallback != null, "endAsyncCallback is null");

            this.storyboard = storyboard;
            this.talkAsyncCallback = talkAsyncCallback;
            this.choiceAsyncCallback = choiceAsyncCallback;
            this.endAsyncCallback = endAsyncCallback;

            storyboard.storyThread = this;
            localStoryThreadPD = new StoryThreadPD(storyboard.guid);

            AddListeners();
            AddCurrentNode(FetchStartNode());
            Play();
        }

        private void AddListeners()
        {
            EventSystem.GetInstance().AddListener(EventID.ApplicationQuit, ApplicationQuitHandler);
        }

        private void RemoveListeners()
        {
            EventSystem.GetInstance().RemoveListener(EventID.ApplicationQuit, ApplicationQuitHandler);
        }

        private void ApplicationQuitHandler(NotificationData data)
        {
            CleanupWhenComplete();
        }

        private void CleanupWhenComplete()
        {
            RemoveListeners();
            storyboard.storyThread = null;
            ResetStoryNodeState();
        }

        private void ResetStoryNodeState()
        {
            var nodes = storyboard.nodes;
            foreach (var node in nodes)
            {
                if (node is StoryNodeBase)
                {
                    (node as StoryNodeBase).state = StoryNodeState.Idle;
                }
            }
        }

        private StoryNodeBase FetchStartNode()
        {
            var nodes = storyboard.nodes;
            foreach (var node in nodes)
            {
                if (node is StartNode)
                {
                    return node as StoryNodeBase;
                }
            }
            return null;
        }

        private void FetchNextNodes()
        {
            var node = GetCurrentNode(0);
            ClearCurrentNodes(StoryNodeState.Played);

            var nextPort = node.GetOutputPort(StoryNodeBase.NEXT_PORT_NAME);
            var connections = nextPort.GetConnections();
            foreach (var connection in connections)
            {
                AddCurrentNode(connection.node as StoryNodeBase);
            }
        }

        private void Play()
        {
            var firstCurrentNode = GetCurrentNode(0);
            if (firstCurrentNode is StartNode)
            {
                FetchNextNodes();
                Play();
            }
            else if (firstCurrentNode is EndNode)
            {
                var endNode = CopyCurrentNodes<EndNode>();
                var asyncHandler = new AsyncHandler<EndNode>(this, endNode);
                endAsyncCallback(asyncHandler);
            }
            else if (firstCurrentNode is LinkNode)
            {
                // Fetch the first connected LinkNode
                bool hasConnectedLinkNode = false;
                for (int i = 0; i < NumberCurrentNodes(); i++)
                {
                    var node = GetCurrentNode(i);
                    var linkNode = node as LinkNode;
                    if (linkNode.Connected())
                    {
                        ClearCurrentNodes(StoryNodeState.Idle);
                        AddCurrentNode(linkNode);
                        hasConnectedLinkNode = true;
                        break;
                    }
                }

                // At least one LinkNode should be connected.
                Utils.Assert(hasConnectedLinkNode, "All LinkNodes are not connected.");

                FetchNextNodes();
                Play();
            }
            else if (firstCurrentNode is ChoiceNode)
            {
                var choiceNodes = CopyCurrentNodes<ChoiceNode>();
                choiceNodes.Sort(SortChoiceNodes);
                var asyncHandler = new AsyncHandler<ChoiceNode>(this, choiceNodes);
                choiceAsyncCallback(asyncHandler);
            }
            else if (firstCurrentNode is TalkNode)
            {
                var talkNodes = CopyCurrentNodes<TalkNode>();
                var asyncHandler = new AsyncHandler<TalkNode>(this, talkNodes);
                talkAsyncCallback(asyncHandler);
            }
            else if (firstCurrentNode is TriggerNode)
            {
                var triggerNode = firstCurrentNode as TriggerNode;
                var triggerPort = triggerNode.GetPort(TriggerNode.TRIGGER_PORT_NAME);
                var connections = triggerPort.GetConnections();
                if (connections.Count == 0) // trigger complete
                {
                    FetchNextNodes();
                    Play();
                }
                else
                {
                    currentTriggerableNodes.Clear();
                    foreach (var connection in connections)
                    {
                        currentTriggerableNodes.Add(connection.node as ITriggerableNode);
                    }
                    PlayTriggerableNodes();
                }
            }
            else
            {
                Utils.Assert(false, "Unexcepted node type");
            }
        }

        private int SortChoiceNodes(ChoiceNode a, ChoiceNode b)
        {
            return
                a.order < b.order ? -1 :
                a.order > b.order ? 1 : 0;
        }

        private void PlayTriggerableNodes()
        {
            currentTriggerableNodes[0].TriggerOn(TriggerableNodeCompleteCallback);
        }

        private void TriggerableNodeCompleteCallback()
        {
            currentTriggerableNodes.RemoveAt(0);

            if (currentTriggerableNodes.Count == 0) // trigger complete
            {
                FetchNextNodes();
                Play();
            }
            else
            {
                PlayTriggerableNodes();
            }
        }

        private List<T> CopyCurrentNodes<T>() where T : StoryNodeBase
        {
            var newNodeList = new List<T>();
            for (int i = 0; i < NumberCurrentNodes(); i++)
            {
                var node = GetCurrentNode(i);
                newNodeList.Add(node as T);
            }
            return newNodeList;
        }
    }
}
