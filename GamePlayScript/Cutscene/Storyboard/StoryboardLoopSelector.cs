using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class StoryboardLoopSelector : SerializableMonoBehaviour<StoryboardLoopSelectorPD>, IStoryboardSelector
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

            if (pd.currentIndex + 1 >= sourceList.Length)
            {
                pd.currentIndex = 0;
            }
            else
            {
                ++pd.currentIndex;
            }
            if (pd.currentIndex < 0 || pd.currentIndex >= sourceList.Length)
            {
                return null;
            }

            if (sourceList[pd.currentIndex] == null)
            {
                return null;
            }

            return sourceList[pd.currentIndex].Value;
        }
    }
}
