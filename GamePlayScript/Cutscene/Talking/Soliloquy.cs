using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;

namespace GameScript.Cutscene
{
    public class Soliloquy : TalkingBase<SoliloquyPD>
    {
        public override void Triggger()
        {
            base.Triggger();

            if (GetSelectedStoryboardName(out string storyboardName))
            {
                new SoliloquyThread(storyboardName);
            }
        }
    }
}
