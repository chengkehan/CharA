using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class Door : SerializableMonoBehaviour<DoorPD>
    {
        [Tooltip("Turning degree of a door is in range of 0 to 90 rather than 0 to 180.\nIf turnDirection is true, it means legal range of turning degree is in possitive local z-axis side.\nIf turnDirection is false, it means legal range of turning degree is in negative local z-axis side.")]
        [SerializeField]
        private bool _turnDirection = false;
        public bool turnDirection
        {
            get
            {
                return _turnDirection;
            }
        }

        [SerializeField]
        private Transform _knob = null;
        private Transform knob
        {
            get
            {
                return _knob;
            }
        }

        [SerializeField]
        private PairsData _keyToDoorPairs = null;
        public PairsData keyToDoorPairs
        {
            get
            {
                return _keyToDoorPairs;
            }
        }

        public Vector3 GetKnobPosition()
        {
            return knob == null ? Vector3.zero : knob.position;
        }

        private Vector3 GetForward()
        {
            return transform == null ? Vector3.zero : transform.forward;
        }

        private Vector3 GetPosition()
        {
            return transform == null ? Vector3.zero : transform.position;
        }

        public bool AtDoorFrontSide(Vector3 rolePos)
        {
            return Vector3.Dot((rolePos - GetPosition()).normalized, GetForward()) > 0 == (turnDirection == false);
        }

        public bool AtDoorBackSide(Vector3 rolePos)
        {
            return !AtDoorFrontSide(rolePos);
        }

        public bool KnobAtRightHandSide(Vector3 roleRight, Vector3 rolePos)
        {
            return Vector3.Dot(roleRight, (GetKnobPosition() - rolePos).normalized) > 0;
        }

        public bool KnobAtLeftHandSide(Vector3 roleRight, Vector3 rolePos)
        {
            return !KnobAtRightHandSide(roleRight, rolePos);
        }

        private void Update()
        {
            UpdateOpenAnimation();
        }

        #region Open Door Animation

        private enum DoorAnimState
        {
            Idle,
            WAITING,
            Opening,
            Opened,
            Closing
        }

        private const float WAITING_TIME = 0.85f;
        private float waitingTime = 0;

        private const float OPENING_TIME = 0.8f;
        private float openingTime = 0;

        private const float OPENED_DURATION_TIME = 1f;
        private float openedDurationTime = 0;

        private const float CLOSING_TIME = 0.25f;
        private float closingTime = 0;

        private DoorAnimState doorAnimState = DoorAnimState.Idle;

        private Vector3 openDoorConstrainedEulerAngles = Vector3.zero;

        public void Open()
        {
            waitingTime = 0;
            openingTime = 0;
            openedDurationTime = 0;
            closingTime = 0;
            doorAnimState = DoorAnimState.WAITING;
            openDoorConstrainedEulerAngles = turnDirection ? new Vector3(0, -70, 0) : new Vector3(0, 70, 0);
        }

        public float HandIKWeight()
        {
            return Mathf.Clamp01(waitingTime / WAITING_TIME);
        }

        private void UpdateOpenAnimation()
        {
            if (doorAnimState == DoorAnimState.WAITING)
            {
                waitingTime += Time.deltaTime;
                if (waitingTime >= WAITING_TIME)
                {
                    doorAnimState = DoorAnimState.Opening;
                }
            }
            else if (doorAnimState == DoorAnimState.Opening)
            {
                transform.localEulerAngles = Vector3.Lerp(Vector3.zero, openDoorConstrainedEulerAngles, openingTime / OPENING_TIME);

                openingTime += Time.deltaTime;
                if (openingTime >= OPENING_TIME)
                {
                    doorAnimState = DoorAnimState.Opened;
                }
            }
            else if (doorAnimState == DoorAnimState.Opened)
            {
                transform.localEulerAngles = openDoorConstrainedEulerAngles;

                openedDurationTime += Time.deltaTime;
                if (openedDurationTime >= OPENED_DURATION_TIME)
                {
                    doorAnimState = DoorAnimState.Closing;
                }
            }
            else if (doorAnimState == DoorAnimState.Closing)
            {
                transform.localEulerAngles = Vector3.Lerp(openDoorConstrainedEulerAngles, Vector3.zero, closingTime / CLOSING_TIME);

                closingTime += Time.deltaTime;
                if (closingTime >= CLOSING_TIME)
                {
                    transform.localEulerAngles = Vector3.zero;
                    doorAnimState = DoorAnimState.Idle;
                }
            }
            else
            {
                // Do nothing
            }
        }

        #endregion

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Color gizmosColor = Gizmos.color;

            {
                Gizmos.color = Color.green;

                Vector3 oPos = transform.position + transform.up;
                Vector3 dir = turnDirection ? -transform.right * 1.5f : -transform.forward * 1.5f;
                Vector3 pPos = oPos + dir;
                float startDegree = 15;
                float endDegree = 91;
                float stepDegree = 15;
                Gizmos.DrawLine(pPos, oPos);
                for (float degree = startDegree; degree < endDegree; degree += stepDegree)
                {
                    var rotation = Quaternion.AngleAxis(degree, transform.up);
                    var npPos = oPos + rotation * dir;
                    Gizmos.DrawLine(pPos, npPos);
                    Gizmos.DrawLine(npPos, oPos);
                    pPos = npPos;
                }
                
            }

            Gizmos.color = gizmosColor;
        }

#endif
    }
}
