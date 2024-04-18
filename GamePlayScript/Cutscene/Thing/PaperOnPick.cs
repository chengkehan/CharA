using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class PaperOnPick : PickableObject.OnPick
    {
        public override void Pick(PickableObject pickableObject)
        {
            UIManager.GetInstance().OpenUI(UIManager.UIName.Paper);
        }
    }
}
