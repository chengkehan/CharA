using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.UI.CardboardBoxUI;

namespace GameScript.Cutscene
{
    public class CardboardBox : SerializableMonoBehaviour<CardboardBoxPD>
    {
        [SerializeField]
        private StorageConfig storageConfig = null;

        protected override void InitializeOnStart()
        {
            base.InitializeOnStart();

            storageConfig.InitializeStorageItems(pd);
        }
    }
}
