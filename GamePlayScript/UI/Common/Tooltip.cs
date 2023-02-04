using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameScript.UI.Common
{
    public class Tooltip : ComponentBase
    {
        [SerializeField]
        private Tooltip_Text_Discard_Eat_Transfer _tooltip_text_discard_eat_transfer = null;
        public Tooltip_Text_Discard_Eat_Transfer tooltip_text_discard_eat_transfer
        {
            get
            {
                return _tooltip_text_discard_eat_transfer;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            tooltip_text_discard_eat_transfer.gameObject.SetActive(false);
        }
    }
}
