using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameScript.UI.Common
{
    public class ScrollerThumb : ComponentBase, IPointerDownHandler, IPointerUpHandler
    {
        private System.Action _onPointerDownCB = null;

        private System.Action _onPointerUpCB = null;

        public void OnPointerDown(PointerEventData eventData)
        {
            GetOnPointerDownCB()?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            GetOnPointerUpCB()?.Invoke();
        }

        public void SetOnPointerDownCB(System.Action cb)
        {
            _onPointerDownCB = cb;
        }

        public System.Action GetOnPointerDownCB()
        {
            return _onPointerDownCB;
        }

        public void SetOnPointerUpCB(System.Action cb)
        {
            _onPointerUpCB = cb;
        }

        public System.Action GetOnPointerUpCB()
        {
            return _onPointerUpCB;
        }
    }
}
