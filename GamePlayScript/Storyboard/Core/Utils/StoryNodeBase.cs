using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using XNode;

namespace StoryboardCore
{
    public class StoryNodeBase : Node
    {
        public const string NEXT_PORT_NAME = "next";

        public const string PREV_PORT_NAME = "prev";

        private StoryNodeState _state = StoryNodeState.Idle;
        public StoryNodeState state
        {
            set
            {
                _state = value;
            }
            get
            {
                return _state;
            }
        }

        [ReadOnly]
        public string guid = null;

        private Storyboard _storyboard = null;
        public Storyboard storyboard
        {
            get
            {
                if (_storyboard == null)
                {
                    _storyboard = graph as Storyboard;
                }
                return _storyboard;
            }
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        public override void OnCloneFromOther()
        {
            base.OnCloneFromOther();

            guid = Guid.NewGuid().ToString();
        }

        protected override void Init()
        {
            base.Init();

            if (string.IsNullOrWhiteSpace(guid))
            {
                guid = Guid.NewGuid().ToString(); 
            }
        }

        public string GetLangauge(string key)
        {
            return storyboard.GetLanguage(key);
        }

        public bool ContainsLanguage(string key)
        {
            return storyboard.ContainsLanguage(key);
        }
    }
}
