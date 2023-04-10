using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using GameScript;

namespace StoryboardCore
{
    [CreateNodeMenu("Game/Thread Value")]
    public class ThreadValueNode : StoryNodeBase, ITriggerableNode
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public TriggerPort prev = null;

        [Tooltip("Set a string value")]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public string i_t = string.Empty;

        [Tooltip("Set a number value")]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public float i_n = 0;

        [Tooltip("Set a boolean value")]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public bool i_b = false;

        [Tooltip("Get a string value")]
        [Output(typeConstraint = TypeConstraint.Strict)]
        public string o_t = string.Empty;

        [Tooltip("Get a number value")]
        [Output(typeConstraint = TypeConstraint.Strict)]
        public float o_n = 0;

        [Tooltip("Set a boolean value")]
        [Output(typeConstraint = TypeConstraint.Strict)]
        public bool o_b = false;

        public string valueName = string.Empty;

        [Tooltip("A persistent value will be saved even if thread is complete, and restored when thread startup next time.\nNon-persistent value will be cleared then thread exits.")]
        public bool persistent = false;

        public override object GetValue(NodePort port)
        {
            if (storyboard.storyThread != null)
            {
                // get value at runtime
                if (port.fieldName == "o_t")
                {
                    return GetThreadValueData().textValue;
                }
                else if (port.fieldName == "o_n")
                {
                    return GetThreadValueData().numberValue;
                }
                else if (port.fieldName == "o_b")
                {
                    return GetThreadValueData().booleanValue;
                }
                else
                {
                    Utils.Assert(false, "Unexpected port name");
                    return 0f;
                }
            }
            else
            {
                // return defualt value when it's not runtime now.
                if (port.fieldName == "o_t")
                {
                    return string.Empty;
                }
                else if(port.fieldName == "o_n")
                {
                    return 0f;
                }
                else if(port.fieldName == "o_b")
                {
                    return false;
                }
                else
                {
                    Utils.Assert(false, "Unexpected port name");
                    return 0f;
                }
            }
        }

        public string GetTextValue()
        {
            return GetValue(GetOutputPort("o_t")) as string;
        }

        public float GetNumberValue()
        {
            return (float)GetValue(GetOutputPort("o_n"));
        }

        public bool GetBooleanValue()
        {
            return (bool)GetValue(GetOutputPort("o_b"));
        }

        public void SetValue()
        {
            if (storyboard.storyThread != null)
            {
                // set value at runtime
                if (GetInputPort("i_t").NumberConnections() > 0)
                {
                    GetThreadValueData().textValue = GetInputValue("i_t", i_t);
                }
                if (GetInputPort("i_n").NumberConnections() > 0)
                {
                    GetThreadValueData().numberValue = GetInputValue("i_n", i_n);
                }
                if (GetInputPort("i_b").NumberConnections() > 0)
                {
                    GetThreadValueData().booleanValue = GetInputValue("i_b", i_b);
                }
            }
            else
            {
                // Do nothing when it's not runtime now.
            }
        }

        public void TriggerOn(Action completeCallback)
        {
            Utils.Assert(completeCallback != null);

            SetValue();

            completeCallback();
        }

        private ThreadValueData GetThreadValueData()
        {
            var threadValueData = persistent ?
                    storyboard.storyThread.persistentStoryThreadPD.GetThreadValueData(valueName) :
                    storyboard.storyThread.localStoryThreadPD.GetThreadValueData(valueName);
            return threadValueData;
        }
    }
}
