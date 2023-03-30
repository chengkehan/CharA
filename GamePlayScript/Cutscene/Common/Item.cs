using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class Item : MonoBehaviour
    {
        private Actor bindingActor = null;

        public void SetPhysics(bool enabled)
        {
            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = GetComponentInChildren<Rigidbody>();
            }
            if (rigidbody != null)
            {
                rigidbody.isKinematic = !enabled;
            }
        }

        public void SetParent(Transform parent)
        {
            if (transform != null)
            {
                transform.SetParent(parent, false);
            }
        }

        public void Binding(Actor actor)
        {
            bindingActor = actor;
        }

        private void LateUpdate()
        {
            if (bindingActor != null && transform != null)
            {
                transform.position = bindingActor.GetRightHandPosition();
                transform.up = bindingActor.GetLeftHandPosition() - bindingActor.GetRightHandPosition();
            }
        }
    }
}
