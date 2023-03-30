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

        public void Triggger()
        {
            Utils.Log("Soliloquy ......");
        }
    }
}
