using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;
using GameScript.Cutscene;

namespace GameScript
{
    public class RoleOperation
    {
        private BaseOperation _operation = null;

        private BaseOperation _secondOperation = null;

        private RoleAnimation _roleAnimation = null;

        // If it's locked, no operation will be executed even if it has a operation.
        // It seems as there is not any operation.
        private bool _isLocked = false;

        public RoleOperation(RoleAnimation roleAnimation)
        {
            _roleAnimation = roleAnimation;
        }

        public void OnSecondOperationUpdate()
        {
            if (GetSecondOperation() != null)
            {
                if (GetSecondOperation().isSecondOperationInterrputed)
                {
                    ClearSecondOperation();
                }
                else
                {
                    GetSecondOperation().OnSecondOperationUpdate();
                }
            }
        }

        public void OnSecondOperationTheEnd()
        {
            if (GetSecondOperation() != null)
            {
                if (GetSecondOperation().isSecondOperationInterrputed == false)
                {
                    GetSecondOperation().OnSecondOperationTheEnd();
                    ClearSecondOperation();
                }
            }
        }

        public RoleAnimation GetRoleAnimation()
        {
            return _roleAnimation;
        }

        public bool GetIsLocked()
        {
            return _isLocked;
        }

        public void SetIsLocked(bool b)
        {
            _isLocked = b;
        }

        public void SetOperation(BaseOperation operation)
        {
            _operation = operation;
            ClearSecondOperation();
        }

        public BaseOperation GetOperation()
        {
            return _operation;
        }

        public BaseOperation GenerateMeetNpcOperation(string npcId)
        {
            if (ActorsManager.GetInstance() != null)
            {
                Actor npc = ActorsManager.GetInstance().GetActor(npcId);
                if (npc != null)
                {
                    Waypoint endWaypoint = GetRoleAnimation().GetWaypointPath().GetNearestMovingWaypoint(npc.roleAnimation.GetMotionAnimator().GetPosition());
                    if (endWaypoint != null)
                    {
                        MeetNpcOperation operation = new MeetNpcOperation(npc.GetId());
                        operation.SetEndWaypoint(endWaypoint);
                        operation.SetRoleAnimation(GetRoleAnimation());
                        return operation;
                    }
                }
            }
            return null;
        }

        public BaseOperation GenerateMovingOperation(Waypoint targetWaypoint)
        {
            if (targetWaypoint != null)
            {
                BaseOperation operation = new MovingOperation();
                operation.SetEndWaypoint(targetWaypoint);
                operation.SetRoleAnimation(GetRoleAnimation());
                return operation;
            }
            else
            {
                return null;
            }
        }

        public BaseOperation GenerateOperation(Vector3 screenPoint, out bool operationIsReady)
        {
            operationIsReady = true;

            if (CameraManager.GetInstance() != null && CameraManager.GetInstance().GetMainCamera() != null)
            {
                Ray ray = CameraManager.GetInstance().GetMainCamera().ScreenPointToRay(screenPoint);
                if (Interactive3DDetector.Detect(ray))
                {
                    {
                        if (Interactive3DDetector.Select<Paper>(out var paper))
                        {
                            operationIsReady = false;
                            var notificationData = new PickupableObjectClickedClickedND();
                            notificationData.paper = paper;
                            EventSystem.GetInstance().Notify(EventID.PickupableObjectClicked, notificationData);
                            return null;
                        }
                    }
                    {
                        if (Interactive3DDetector.Select<CardboardBox>(out var cardboardBox))
                        {
                            operationIsReady = false;
                            var notificationData = new PickupableObjectClickedClickedND();
                            notificationData.cardboardBox = cardboardBox;
                            EventSystem.GetInstance().Notify(EventID.PickupableObjectClicked, notificationData);
                            return null;
                        }
                    }
                    {
                        if (Interactive3DDetector.Select<Actor>(out var actor))
                        {
                            if (actor.isHero)
                            {
                                BaseOperation operation = new ClickHeroOperation();
                                operation.stopMovingBeforeExecute = true;
                                return operation;
                            }
                        }
                    }
                    {
                        if (Interactive3DDetector.Select<Actor>(out var actor))
                        {
                            if (actor.isHero == false)
                            {
                                if (Interactive3DDetector.Select<WaypointInductor>(out var _))
                                {
                                    Vector3 hitPoint = Interactive3DDetector.GetHitPointOfRay();
                                    if (Interactive3DDetector.TryGetHitBoxColliderBounds(out Bounds localBounds, out Matrix4x4 worldToLocalBounds))
                                    {
                                        Waypoint endWaypoint = GetRoleAnimation().GetWaypointPath().GetNearestMovingWaypointInBounds(hitPoint, localBounds, worldToLocalBounds);
                                        BaseOperation operation = new MeetNpcOperation(actor.GetId());
                                        operation.SetEndWaypoint(endWaypoint);
                                        operation.SetRoleAnimation(GetRoleAnimation());
                                        return operation;
                                    }
                                }
                            }
                        }
                    }
                    {
                        if (Interactive3DDetector.Select<WaypointInductor>(out var _))
                        {
                            Vector3 hitPoint = Interactive3DDetector.GetHitPointOfRay();
                            if (Interactive3DDetector.TryGetHitBoxColliderBounds(out Bounds localBounds, out Matrix4x4 worldToLocalBounds))
                            {
                                Waypoint endWaypoint = GetRoleAnimation().GetWaypointPath().GetNearestMovingWaypointInBounds(hitPoint, localBounds, worldToLocalBounds);
                                BaseOperation operation = new MovingOperation();
                                operation.SetEndWaypoint(endWaypoint);
                                operation.SetRoleAnimation(GetRoleAnimation());
                                return operation;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public BaseOperation GenerateLadderMotionOperation(Waypoint startWaypoint, BaseOperation otherOperation)
        {
            if (otherOperation == null)
            {
                return null;
            }
            LadderMotionOperation operation = new LadderMotionOperation();
            operation.SetEndWaypoint(otherOperation.GetEndWaypoint());
            operation.SetRoleAnimation(GetRoleAnimation());
            operation.SetStartWaypoint(startWaypoint);
            return operation;
        }

        public bool ExecuteOperation()
        {
            if (_operation != null && GetIsLocked() == false)
            {
                return _operation.ExecuteOperation();
            }
            else
            {
                return false;
            }
        }

        public bool HasOperation()
        {
            return _operation != null && GetIsLocked() == false;
        }

        public void ClearOperation()
        {
            SetSecondOperation(_operation);
            _operation = null;
        }

        private void SetSecondOperation(BaseOperation operation)
        {
            _secondOperation = operation;
        }

        private BaseOperation GetSecondOperation()
        {
            return _secondOperation;
        }

        private void ClearSecondOperation()
        {
            _secondOperation = null;
        }

        private T FetchComponent<T>(Transform transform)
            where T : MonoBehaviour
        {
            if (transform != null)
            {
                T component = transform.gameObject.GetComponent<T>();
                if (component == null && transform.parent != null)
                {
                    component = transform.parent.gameObject.GetComponent<T>();
                }
                return component;
            }
            return default(T);
        }
    }
}
