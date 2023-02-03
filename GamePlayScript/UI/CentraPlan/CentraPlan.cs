using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using CUI = GameScript.UI.Common;

namespace GameScript.UI.CentraPlan
{
    public class CentraPlan : CUI.UIBase
    {
        [SerializeField]
        private Button _closeButton = null;
        private Button closeButton
        {
            get
            {
                return _closeButton;
            }
        }

        [SerializeField]
        private HeroPanel _heroPanel = null;
        private HeroPanel heroPanel
        {
            get
            {
                return _heroPanel;
            }
        }

        private void Start()
        {
            closeButton.onClick.AddListener(CloseHandler);
            heroPanel.AlignToHero();
        }

        private void CloseHandler()
        {
            UIManager.GetInstance().CloseUI(UIManager.UIName.CentraPlan);
        }
    }
}
