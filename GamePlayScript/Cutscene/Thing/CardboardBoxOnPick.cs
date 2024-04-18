using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.UI.CardboardBoxUI;

namespace GameScript.Cutscene
{
    public class CardboardBoxOnPick : PickableObject.OnPick
    {
        public override void Pick(PickableObject pickableObject)
        {
            var cardboardBox = pickableObject.GetComponent<CardboardBox>();
            var actor = ActorsManager.GetInstance().GetHeroActor();
            actor.roleAnimation.GetMotionAnimator().SetSoloState(SoloSM.Transition.StandingToCrouched);
            UIManager.GetInstance().OpenUI(UIManager.UIName.CardboardBox,
            () =>
            {
                var cardboardBoxUI = UIManager.GetInstance().GetUI<CardboardBoxUI>(UIManager.UIName.CardboardBox);
                cardboardBoxUI.Initialize(cardboardBox);
            },
            () =>
            {
                actor.roleAnimation.GetMotionAnimator().SetSoloState(SoloSM.Transition.CrouchedToStanding);
            });
        }
    }
}
