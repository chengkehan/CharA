using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameScript.UI.MenuBar
{
    public class MenuBar : MonoBehaviour
    {
        [SerializeField]
        private Button _backpackButton = null;
        private Button backpackButton
        {
            get
            {
                return _backpackButton;
            }
        }
    }
}
