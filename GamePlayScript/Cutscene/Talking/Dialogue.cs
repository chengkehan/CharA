using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.UI.Talking;

namespace GameScript.Cutscene
{
    public class Dialogue : TalkingBase<DialoguePD>
    {
        public override void Triggger()
        {
            base.Triggger();

            if (GetSelectedStoryboardName(out string storyboardName))
            {
                UIManager.GetInstance().OpenTalkingUI(storyboardName);
            }
        }
    }
}
