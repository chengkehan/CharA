using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;

namespace GameScript.Cutscene
{
    public class PickableObject : MonoBehaviour
    {
        [Tooltip("Waypoints in this bounds to make sure role can walk up to here")]
        [SerializeField]
        private BoundsComponent _waypointsBounds = new BoundsComponent();
        public BoundsComponent waypointsBounds
        {
            get
            {
                return _waypointsBounds;
            }
        }

        public Vector3 GetPosition()
        {
            return transform == null ? Vector3.zero : transform.position;
        }

        public void Pick()
        {
            var onPick = GetComponent<OnPick>();
            if (onPick != null)
            {
                onPick.Pick(this);
            }
        }

        public abstract class OnPick : MonoBehaviour
        {
            public abstract void Pick(PickableObject pickableObject);
        }
    }
}
