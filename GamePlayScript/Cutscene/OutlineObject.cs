using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class OutlineObject : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _outlineGos = null;

        private void Awake()
        {
            HideOutline();
        }

        public void ShowOutline()
        {
            OutlineGosVisible(true);
        }

        public void HideOutline()
        {
            OutlineGosVisible(false);
        }

        private void OutlineGosVisible(bool b)
        {
            if (_outlineGos != null)
            {
                foreach (var item in _outlineGos)
                {
                    if (item != null && item.activeSelf != b)
                    {
                        item.SetActive(b);
                    }
                }
            }
        }
    }
}
