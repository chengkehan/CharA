using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GameScript.UI.Common;

namespace GameScript.UI.CentraPlan
{
    public class Settings : MonoBehaviour
    {
        public GButton saveButton = null;

        private void Start()
        {
            saveButton.SetClickedCB(()=>
            {
                DataCenter.GetInstance().Save();
            });
        }
    }
}
