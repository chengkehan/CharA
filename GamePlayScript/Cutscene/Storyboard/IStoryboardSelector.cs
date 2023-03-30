using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;

namespace GameScript.Cutscene
{
    public interface IStoryboardSelector
    {
        IStoryboardConfig Select(InterfaceReference<IStoryboardConfig, MonoBehaviour>[] sourceList);
    }
}
