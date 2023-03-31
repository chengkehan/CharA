using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class StoryboardOneSelector : SerializableMonoBehaviour<StoryboardOneSelectorPD>, IStoryboardSelector
    {
        public IStoryboardConfig Select(InterfaceReference<IStoryboardConfig, MonoBehaviour>[] sourceList)
        {
            if (sourceList == null)
            {
                return null;
            }
            if (sourceList.Length == 0)
            {
                return null;
            }
            if (sourceList[0] == null)
            {
                return null;
            }
            return sourceList[0].Value;
        }
    }
}
