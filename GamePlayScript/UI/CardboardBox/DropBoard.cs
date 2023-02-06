using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript.UI.CardboardBoxUI
{
    public class DropBoard : MonoBehaviour
    {
        public Action<Collider> onTriggerEnter = null;

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }
    }
}
