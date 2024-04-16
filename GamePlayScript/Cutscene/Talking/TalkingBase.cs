using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;

namespace GameScript.Cutscene
{
    public abstract class TalkingBase<TPD> : SerializableMonoBehaviour<TPD>, ITriggerTarget
        where TPD : SerializableMonoBehaviourPD, new()
    {
        [SerializeField]
        protected InterfaceReference<IStoryboardConfig, MonoBehaviour>[] sourceList = null;

        [SerializeField]
        protected InterfaceReference<IStoryboardSelector, MonoBehaviour> selector = null;

        public virtual void Triggger()
        {
            // Do nothing
        }

        protected bool GetSelectedStoryboardName(out string storyboardName)
        {
            storyboardName = null;
            if (selector != null && selector.Value != null)
            {
                var item = selector.Value.Select(sourceList);
                if (item != null)
                {
                    storyboardName = item.storyboardName;
                    return true;
                }
            }
            return false;
        }
    }
}
