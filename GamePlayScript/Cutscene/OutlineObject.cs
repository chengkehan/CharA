using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class OutlineObject : MonoBehaviour
    {
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
                    Utils.Log("clicked", gameObject);
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
