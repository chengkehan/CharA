using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class StoryboardConfig : MonoBehaviour, IStoryboardConfig
    {
        [SerializeField]
        [StoryboardNameAttribute]
        private string _storyboardName = null;
        public string storyboardName
        {
            get
            {
                return _storyboardName;
            }
        }
    }
}
