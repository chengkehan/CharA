using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameScript.WaypointSystem;
using System;
using GameScript.Cutscene;

namespace GameScript.UI.HUD
{
    public class BreakWallHUD : Project3DHUD
    {
        [SerializeField]
        private Button _button = null;
        private Button button
        {
            get
            {
                return _button;
            }
        }

        [SerializeField]
        private Image _progressImage = null;
        private Image progressImage
        {
            get
            {
                return _progressImage;
            }
        }

        private BreakWall _breakWall = null;
        public BreakWall breakWall
        {
            set
            {
                _breakWall = value;
            }
            get
            {
                return _breakWall;
            }
        }

        protected override void Start()
        {
            base.Start();

            button.onClick.AddListener(ButtonOnClickHandler);
        }

        protected override void Update()
        {
            base.Update();

            progressImage.fillAmount = 1 - breakWall.pd.health / breakWall.totalHealth;
        }

        private void ButtonOnClickHandler()
        {
            var notificationData = new BreakWallHUDClickedND();
            notificationData.breakWall = breakWall;
            EventSystem.GetInstance().Notify(EventID.BreakWallHUDClicked, notificationData);
        }

        protected override void UpdateVisibleByDistanceFromHero_Internal(Vector3 heroPosition)
        {
            button.gameObject.SetActive(breakWall.InHUDBounds(heroPosition) && DataCenter.query.IsWallBreaked(breakWall.pd) == false);
        }

        protected override Vector3 Get3DPosition()
        {
            return breakWall == null ? Vector3.zero : breakWall.GetPosition();
        }
    }
}
