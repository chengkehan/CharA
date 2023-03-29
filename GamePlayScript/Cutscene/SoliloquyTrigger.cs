using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class SoliloquyTrigger : SerializableMonoBehaviour<SoliloquyTriggerPD>, IBoundsTriggerTarget
    {
        public void Triggger()
        {
            Utils.Log("Soliloquy ......");
        }
    }
}
