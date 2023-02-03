using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class Paper : SerializableMonoBehaviour<PaperPD>
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
    }
}
