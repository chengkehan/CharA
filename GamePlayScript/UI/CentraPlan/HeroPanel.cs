using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using CUI = GameScript.UI.Common;

namespace GameScript.UI.CentraPlan
{
    public class HeroPanel : MonoBehaviour
    {
        public void AlignToHero()
        {
            if (ActorsManager.GetInstance() != null)
            {
                var heroActor = ActorsManager.GetInstance().GetHeroActor();
                if (heroActor != null)
                {
                    var heroContainer = GetComponent<RectTransform>();
                    var heroWPos = heroActor.roleAnimation.GetMotionAnimator().GetPosition();
                    heroWPos.y += 1;
                    if (CUI.ComponentBase.ConvertWorldPositionToLocalPoint(heroWPos, true, heroContainer.parent.GetComponent<RectTransform>(), out var localPoint))
                    {
                        heroContainer.anchoredPosition = localPoint;
                    }
                }
            }
        }
    }
}
