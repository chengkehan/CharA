using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class OutlineObject : MonoBehaviour
    {
        public static bool TurnOnOff(GameObject targetGo, bool enabled)
        {
            if (targetGo != null)
            {
                if (targetGo.GetComponent<OutlineObject>() != null)
                {
                    targetGo.GetComponent<OutlineObject>().enabled = enabled;
                    return true;
                }
                if (targetGo.GetComponentInChildren<OutlineObject>(true) != null)
                {
                    targetGo.GetComponentInChildren<OutlineObject>(true).enabled = enabled;
                    return true;
                }
            }
            return false;
        }

        public static bool OnClick(GameObject targetGo, Action clickedCB)
        {
            if (targetGo != null && clickedCB != null)
            {
                if (targetGo.GetComponent<OutlineObject>() != null)
                {
                    targetGo.GetComponent<OutlineObject>().clickedCB = clickedCB;
                    return true;
                }
                if (targetGo.GetComponentInChildren<OutlineObject>() != null)
                {
                    targetGo.GetComponentInChildren<OutlineObject>().clickedCB = clickedCB;
                    return true;
                }
            }
            return false;
        }

        private Action clickedCB = null;

        [SerializeField]
        private GameObject[] _outlineGos = null;

        private bool isMouseDown = false;

        private void Awake()
        {
            HideOutline();
        }

        private void OnDisable()
        {
            if (Interactive3DDetector.recentOutlineObject == this)
            {
                HideOutline();
                Interactive3DDetector.RecentOutlineObjectHide();
            }
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
