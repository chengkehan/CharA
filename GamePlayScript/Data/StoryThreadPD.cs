using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using StoryboardCore;

namespace GameScript
{
    [Serializable]
    public class StoryThreadPD
    {
        [SerializeField]
        private string _storyboardGuid = null;
        public string storyboardGuid
        {
            private set
            {
                _storyboardGuid = value;
            }
            get
            {
                return _storyboardGuid;
            }
        }

        [SerializeField]
        private SerializableDictionary<string/*value name*/, ThreadValueData> _allThreadValueData = new SerializableDictionary<string, ThreadValueData>();
        private SerializableDictionary<string, ThreadValueData> allThreadValueData
        {
            get
            {
                return _allThreadValueData;
            }
        }

        public StoryThreadPD(string storyboardGuid)
        {
            this.storyboardGuid = storyboardGuid;
        }

        public ThreadValueData GetThreadValueData(string valueName)
        {
            Utils.Assert(string.IsNullOrWhiteSpace(valueName) == false, "value name of threadValue can't be empty");

            if (allThreadValueData.TryGetValue(valueName, out var threadValueData) == false)
            {
                threadValueData = new ThreadValueData(valueName);
                allThreadValueData.Add(valueName, threadValueData);
            }
            return threadValueData;
        }
    }
}
