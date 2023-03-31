using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;

namespace GameScript.Cutscene
{
    public class Soliloquy : SerializableMonoBehaviour<SoliloquyPD>, IBoundsTriggerTarget
    {
        [SerializeField]
        private InterfaceReference<IStoryboardConfig, MonoBehaviour>[] sourceList = null;

        [SerializeField]
        private InterfaceReference<IStoryboardSelector, MonoBehaviour> selector = null;

        public void Triggger()
        {
            Utils.Log("Soliloquy ......");

            if (selector != null && selector.Value != null)
            {
                var item = selector.Value.Select(sourceList);
                Utils.Log(item);
            }
        }
    }
}
