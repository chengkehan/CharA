using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class RoleHandIK : MonoBehaviour
    {
        private Vector3 _targetPosition = Vector3.zero;
        public Vector3 targetPosition
        {
            set
            {
                _targetPosition = value;
            }
            get
            {
                return _targetPosition;
            }
        }

        private float _weight = 1;
        public float weight
        {
            set
            {
                _weight = value;
            }
            get
            {
                return _weight;
            }
        }

        private bool _isLeftHand = false;
        public bool isLeftHand
        {
            set
            {
                _isLeftHand = value;
            }
            get
            {
                return _isLeftHand;
            }
        }

        private Animator animator = null;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK()
        {
            if (animator == null)
            {
                return;
            }

            var whichHand = isLeftHand ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;
            animator.SetIKPosition(whichHand, targetPosition);
            animator.SetIKPositionWeight(whichHand, weight);
        }
    }
}
