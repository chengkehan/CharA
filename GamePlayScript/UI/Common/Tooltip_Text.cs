using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameScript.UI.Common
{
    public class Tooltip_Text : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text = null;
        private TMP_Text text
        {
            get
            {
                return _text;
            }
        }

        public void SetText(string str)
        {
            gameObject.SetActive(true);
            text.text = str == null ? string.Empty : str;
        }
    }
}
