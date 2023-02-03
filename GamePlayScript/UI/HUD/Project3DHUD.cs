using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.UI.Common;

namespace GameScript.Cutscene
{
    public abstract class Project3DHUD : ComponentBase
    {
        public void UpdatePositionAndVisible()
        {
            Project3DPositionTo2DPoint();
            UpdateVisibleByDistanceFromHero();
        }

        public void UpdateVisibleByDistanceFromHero()
        {
            if (ActorsManager.GetInstance() != null && ActorsManager.GetInstance().GetHeroActor() != null)
            {
                var heroPosition = ActorsManager.GetInstance().GetHeroActor().roleAnimation.GetMotionAnimator().GetPosition();
                UpdateVisibleByDistanceFromHero_Internal(heroPosition);
            }
        }

        protected abstract void UpdateVisibleByDistanceFromHero_Internal(Vector3 heroPosition);

        protected virtual void Update()
        {
            UpdatePositionAndVisible();
        }

        protected abstract Vector3 Get3DPosition();

        private void Project3DPositionTo2DPoint()
        {
            if (transform != null && transform.parent != null)
            {
                var container = transform.parent.GetComponent<RectTransform>();
                if (container != null)
                {
                    if (ConvertWorldPositionToLocalPoint(Get3DPosition(), true, container, out var localPoint))
                    {
                        GetComponent<RectTransform>().anchoredPosition = localPoint;
                    }
                }
            }
        }
    }
}
