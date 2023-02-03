using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    public abstract class BaseOperation
    {
        private Waypoint _endWaypoint = null;

        private RoleAnimation _roleAnimation = null;

        private bool _stopMovingBeforeExecute = false;
        public bool stopMovingBeforeExecute
        {
            set
            {
                _stopMovingBeforeExecute = value;
            }
            get
            {
                return _stopMovingBeforeExecute;
            }
        }

        private bool _isSecondOperationInterrupted = false;
        public bool isSecondOperationInterrputed
        {
            protected set
            {
                _isSecondOperationInterrupted = value;
            }
            get
            {
                return _isSecondOperationInterrupted;
            }
        }

        public Waypoint GetEndWaypoint()
        {
            return _endWaypoint;
        }

        public void SetEndWaypoint(Waypoint endWaypoint)
        {
            _endWaypoint = endWaypoint;
        }

        public void SetRoleAnimation(RoleAnimation roleAnimation)
        {
            _roleAnimation = roleAnimation;
        }

        public RoleAnimation GetRoleAnimation()
        {
            return _roleAnimation;
        }

        public abstract bool ExecuteOperation();

        // execute this update util another operation is execute or it stop itself.
        public abstract void OnSecondOperationUpdate();

        // execute this method when nothing to do
        public abstract void OnSecondOperationTheEnd();
    }
}
