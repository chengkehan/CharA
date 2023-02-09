using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class OutlineObject : MonoBehaviour
    {
        public static bool OnClick(GameObject targetGo, Action clickedCB)
        {
            if (targetGo != null && clickedCB != null && targetGo.GetComponent<OutlineObject>() != null)
            {
                targetGo.GetComponent<OutlineObject>().clickedCB = clickedCB;
                return true;
            }
            else
            {
                return false;
            }
        }

        private Action clickedCB = null;

        [SerializeField]
        private GameObject[] _outlineGos = null;

        private bool isMouseDown = false;

        private void Awake()
        {
            HideOutline();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Interactive3DDetector.recentOutlineObject == this)
                {
                    isMouseDown = true;
                }
            }
            if (isMouseDown && Input.GetMouseButtonUp(0))
            {
                if (Interactive3DDetector.recentOutlineObject == this)
                {
                    clickedCB?.Invoke();
                }
                isMouseDown = false;
            }
        }

        public void ShowOutline()
        {
            OutlineGosVisible(true);
        }

        public void HideOutline()
        {
            isMouseDown = false;
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
