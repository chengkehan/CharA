using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameScript.UI.Common
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField]
        private Tooltip_Text _tooltip_text = null;
        public Tooltip_Text tooltip_text
        {
            get
            {
                return _tooltip_text;
            }
        }

        [SerializeField]
        private Tooltip_Text_Discard _tooltip_text_discard = null;
        public Tooltip_Text_Discard tooltip_text_discard
        {
            get
            {
                return _tooltip_text_discard;
            }
        }

        [SerializeField]
        private Tooltip_Text_Discard_Eat _tooltip_text_discard_eat = null;
        public Tooltip_Text_Discard_Eat tooltip_text_discard_eat
        {
            get
            {
                return _tooltip_text_discard_eat;
            }
        }

        [SerializeField]
        private Tooltip_Text_Discard_Eat_Transfer _tooltip_text_discard_eat_transfer = null;
        public Tooltip_Text_Discard_Eat_Transfer tooltip_text_discard_eat_transfer
        {
            get
            {
                return _tooltip_text_discard_eat_transfer;
            }
        }

        private void Awake()
        {
            tooltip_text.gameObject.SetActive(false);
            tooltip_text_discard.gameObject.SetActive(false);
            tooltip_text_discard_eat.gameObject.SetActive(false);
            tooltip_text_discard_eat_transfer.gameObject.SetActive(false);
        }
    }
}
