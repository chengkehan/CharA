using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    public class RoleAnimation : MonoBehaviour
    {
        private static List<RoleAnimation> allRoleAnimations = new List<RoleAnimation>();

        private MotionAnimator motionAnimator = null;

        private RoleOperation operation = null;

        private WaypointPath waypointPath = null;

        private SetOnceObject<Actor> _actor = new SetOnceObject<Actor>();
        public SetOnceObject<Actor> actor
        {
            get
            {
                return _actor;
            }
        }

        public MotionAnimator GetMotionAnimator()
        {
            return motionAnimator;
        }

        public RoleOperation GetOperation()
        {
            return operation;
        }

        public WaypointPath GetWaypointPath()
        {
            return waypointPath;
        }

        protected virtual void Awake()
        {
            allRoleAnimations.Add(this);
            motionAnimator = new MotionAnimator(this);
            operation = new RoleOperation(this);
            waypointPath = new WaypointPath(this);
        }

        protected virtual void Update()
        {
            motionAnimator.Update();
        }

        protected virtual void OnDestroy()
        {
            allRoleAnimations.Remove(this);
        }
    }
}
