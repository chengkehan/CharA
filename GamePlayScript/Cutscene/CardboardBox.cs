using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class CardboardBox : PickableObject<CardboardBoxPD>
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
