using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameScript.UI.Common;

namespace GameScript.UI.Talking
{
    public class NextStep : ComponentBase
    {
        public GButton button = null;

        public void SetClickedCB(Action action)
        {
            button.SetClickedCB(action);
        }
    }
}
