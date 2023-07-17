using System.Collections;
using GameScript.Cutscene;
using UnityEngine;
using GameScript.UI.Talking;
using GameScript.UI.CardboardBoxUI;
using System;

namespace GameScript
{
    public class HeroBrain : RoleAnimation
    {
        private bool isMouseButtonDown = false;

        protected override void Awake()
        {
            base.Awake();

            AddListeners();
        }

        protected override void Update()
        {
            if (DataCenter.GetInstance().bloackboard.heroSoloAndMuteOthers)
            {
                return;
            }
            if (GetMotionAnimator().ContainsState(MotionAnimator.State.Solo))
            {
                return;
            }

            base.Update();

            if (UIManager.GetInstance().eventSystemEnable &&
                UIManager.GetInstance().IsUIBreakInteractiveScene() == false &&
                UIManager.GetInstance().IsMouseOverUI() == false)
            {
                Interactive3DDetector.DetectOutlineObject();

                if (Input.GetMouseButtonDown(0))
                {
                    isMouseButtonDown = true;
                }
                if (Input.GetMouseButtonUp(0) && isMouseButtonDown)
                {
                    isMouseButtonDown = false;

                    var operation = GetOperation().GenerateOperation(Input.mousePosition, out bool operationIsReady);
                    if (operationIsReady)
                    {
                        GetOperation().SetOperation(operation);
                    }
                }
            }

            UpateBreakWall();
            UpdateDropItemToSceneWhenSomeAnimations();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            RemoveListeners();
        }

        #region Listeners

        private void AddListeners()
        {
            EventSystem.GetInstance().AddListener(EventID.MeetNpc, MeetNpcHandler);
            EventSystem.GetInstance().AddListener(EventID.BreakWallHUDClicked, BreakWallClickedHUDHandler);
            EventSystem.GetInstance().AddListener(EventID.MoveToWaypointsEnd, MoveToWaypointsEndHanlder);
            EventSystem.GetInstance().AddListener(EventID.WannaOpenLockedDoor, WannaOpenLockedDoorHandler);
            EventSystem.GetInstance().AddListener(EventID.SceneItemHUDClicked, SceneItemHUDClickedHandler);
            EventSystem.GetInstance().AddListener(EventID.NewOperation, NewOperationHandler);
            EventSystem.GetInstance().AddListener(EventID.NoWayToPoint, NoWayToPointHandler);
            EventSystem.GetInstance().AddListener(EventID.StopShakeHead, StopShakeHeadHandler);
            EventSystem.GetInstance().AddListener(EventID.PickupableObjectClicked, PickupableObjectClickedHandler);
        }

        private void RemoveListeners()
        {
            EventSystem.GetInstance().RemoveListener(EventID.MeetNpc, MeetNpcHandler);
            EventSystem.GetInstance().RemoveListener(EventID.BreakWallHUDClicked, BreakWallClickedHUDHandler);
            EventSystem.GetInstance().RemoveListener(EventID.MoveToWaypointsEnd, MoveToWaypointsEndHanlder);
            EventSystem.GetInstance().RemoveListener(EventID.WannaOpenLockedDoor, WannaOpenLockedDoorHandler);
            EventSystem.GetInstance().RemoveListener(EventID.SceneItemHUDClicked, SceneItemHUDClickedHandler);
            EventSystem.GetInstance().RemoveListener(EventID.NewOperation, NewOperationHandler);
            EventSystem.GetInstance().RemoveListener(EventID.NoWayToPoint, NoWayToPointHandler);
            EventSystem.GetInstance().RemoveListener(EventID.StopShakeHead, StopShakeHeadHandler);
            EventSystem.GetInstance().RemoveListener(EventID.PickupableObjectClicked, PickupableObjectClickedHandler);
        }

        private void NewOperationHandler(NotificationData _data)
        {
            var data = _data as NewOperationND;
            if (data != null && data.roleID == actor.o.GetId())
            {
                NewOperationHandler_BreakWall(data);
                NewOperationHandler_PickupableObject(data);
            }
        }

        private void MoveToWaypointsEndHanlder(NotificationData _data)
        {
            var data = _data as MoveToWaypointsEndND;
            if (data != null && data.roleID == actor.o.GetId())
            {
                MoveToWaypointsEndHanlder_BreakWall(data);
                MoveToWaypointsEndHanlder_SceneItemHUDClicked(data);
                MoveToWaypointsEndHandler_PickupableObject(data);
            }
        }

        #endregion

        #region PickupableObject

        private PickupableObjectClickedClickedND _pickupableObjectND = null;

        private Coroutine _openPickupableObjectDelay = null;

        private IEnumerator OpenPickupableObjectDelayCoroutine(PickupableObjectClickedClickedND data, float delaySeconds)
        {
            // Waiting for turning complete
            yield return new WaitForSeconds(delaySeconds);

            if (data.paper != null)
            {
                UIManager.GetInstance().OpenUI(UIManager.UIName.Paper);
            }
            if (data.cardboardBox != null)
            {
                GetMotionAnimator().SetSoloState(SoloSM.Transition.StandingToCrouched);
                UIManager.GetInstance().OpenUI(UIManager.UIName.CardboardBox,
                ()=>
                {
                    var cardboardBoxUI = UIManager.GetInstance().GetUI<CardboardBoxUI>(UIManager.UIName.CardboardBox);
                    cardboardBoxUI.Initialize(data.cardboardBox);
                },
                () =>
                {
                    GetMotionAnimator().SetSoloState(SoloSM.Transition.CrouchedToStanding);
                });
            }
        }

        private void StopOpenPickupableObjectDelay()
        {
            if (_openPickupableObjectDelay != null)
            {
                StopCoroutine(_openPickupableObjectDelay);
                _openPickupableObjectDelay = null;
            }
        }

        private void NewOperationHandler_PickupableObject(NewOperationND data)
        {
            StopOpenPickupableObjectDelay();
        }

        private void PickupableObjectClickedHandler(NotificationData _data)
        {
            var data = _data as PickupableObjectClickedClickedND;
            if (data != null)
            {
                GetPickupableObjectData(data, out var position, out var bounds);
                var waypoint = GetWaypointPath().GetNearestMovingWaypointInBounds(position, bounds, Matrix4x4.identity);
                if (waypoint != null)
                {
                    _pickupableObjectND = data;

                    var operation = GetOperation().GenerateMovingOperation(waypoint);
                    GetOperation().SetOperation(operation);
                }
            }
        }

        private void MoveToWaypointsEndHandler_PickupableObject(MoveToWaypointsEndND data)
        {
            if (_pickupableObjectND != null)
            {
                GetPickupableObjectData(_pickupableObjectND, out var position, out var bounds);
                var waypoint = GetWaypointPath().GetNearestMovingWaypointInBounds(position, bounds, Matrix4x4.identity);
                if (waypoint == data.endWaypoint)
                {
                    GetMotionAnimator().StopShakeHead(false);

                    float delaySeconds = 0;
                    if (GetMotionAnimator().AtRightHandSide(position))
                    {
                        delaySeconds = 1;
                        GetMotionAnimator().SetState(MotionAnimator.State.Turning, MotionAnimator.State.DirectionRight);
                    }
                    if (GetMotionAnimator().AtLeftHandSide(position))
                    {
                        delaySeconds = 1;
                        GetMotionAnimator().SetState(MotionAnimator.State.Turning, MotionAnimator.State.DirectionLeft);
                    }

                    StopOpenPickupableObjectDelay();
                    _openPickupableObjectDelay = StartCoroutine(OpenPickupableObjectDelayCoroutine(_pickupableObjectND, delaySeconds));
                }
            }
            _pickupableObjectND = null;
        }

        private void GetPickupableObjectData(PickupableObjectClickedClickedND data, out Vector3 position, out Bounds bounds)
        {
            position = Vector3.zero;
            bounds = new Bounds(Vector3.zero, Vector3.one);

            if (data != null)
            {
                if (data.paper != null)
                {
                    position = data.paper.GetPosition();
                    bounds = data.paper.waypointsBounds.bounds;
                }
                if (data.cardboardBox != null)
                {
                    position = data.cardboardBox.GetPosition();
                    bounds = data.cardboardBox.waypointsBounds.bounds;
                }
            }
        }

        #endregion

        #region No way to point talking bubble

        private Coroutine _shakeHeadTalkingBubble = null;

        private void NoWayToPointHandler(NotificationData _data)
        {
            var data = _data as NoWayToPointND;
            if (data != null && data.roleID == actor.o.GetId())
            {
                if (_shakeHeadTalkingBubble == null)
                {
                    _shakeHeadTalkingBubble = StartCoroutine(ShakeHeadTalkingBubbleCoroutine());
                }
            }
        }

        private IEnumerator ShakeHeadTalkingBubbleCoroutine()
        {
            yield return new WaitForSeconds(0.2f);
            actor.o.TalkingBubble("no_way_to_point");
            _shakeHeadTalkingBubble = null;
        }

        private void StopShakeHeadHandler(NotificationData _data)
        {
            var data = _data as StopShakeHeadND;
            if (data != null && data.roleID == actor.o.GetId())
            {
                if (_shakeHeadTalkingBubble != null)
                {
                    StopCoroutine(_shakeHeadTalkingBubble);
                    _shakeHeadTalkingBubble = null;
                }
            }
        }

        #endregion

        #region Scene item hud clicked

        private string _sceneItemHUDClicked_ItemGUID = null;

        private void ResetSceneItemHUDClicked()
        {
            _sceneItemHUDClicked_ItemGUID = null;
        }

        private void SceneItemHUDClickedHandler(NotificationData _data)
        {
            var data = _data as SceneItemHUDClickedND;
            if (data != null)
            {
                _sceneItemHUDClicked_ItemGUID = data.itemGUID;
                var sceneItemPD = Scene.GetInstance().pd.GetSceneItemPD(data.itemGUID);
                var nearestWaypoint = GetWaypointPath().GetNearestMovingWaypoint(sceneItemPD.worldPosition);
                if (nearestWaypoint != null)
                {
                    var operation = GetOperation().GenerateMovingOperation(nearestWaypoint);
                    GetOperation().SetOperation(operation);
                }
            }
        }

        private void MoveToWaypointsEndHanlder_SceneItemHUDClicked(MoveToWaypointsEndND data)
        {
            if (_sceneItemHUDClicked_ItemGUID != null)
            {
                var sceneItemPD = Scene.GetInstance().pd.GetSceneItemPD(_sceneItemHUDClicked_ItemGUID);
                if (Vector3.Distance(sceneItemPD.worldPosition, data.endWaypoint.GetPosition()) < DataCenter.define.SceneItemPickupRange)
                {
                    ModeratorUtils.PickUpSceneItem(actor.o.guid, _sceneItemHUDClicked_ItemGUID);
                }

                ResetSceneItemHUDClicked();
            }
        }

        #endregion

        #region Wanna open locked door

        private void WannaOpenLockedDoorHandler(NotificationData _data)
        {
            var data = _data as WannaOpenLockedDoorND;
            if (data != null)
            {
                var actor = ActorsManager.GetInstance().GetActorByGUID(data.actorGUID);
                StartCoroutine(SendOpenLockedDoorTalkingBubbleDelay(actor));
            }
        }

        private IEnumerator SendOpenLockedDoorTalkingBubbleDelay(Actor actor)
        {
            yield return new WaitForSeconds(1f);

            actor.TalkingBubble("open_locked_door_talking_rubble");
        }

        #endregion

        #region Meet npc

        private void MeetNpcHandler(NotificationData data)
        {
            var obj = data as MeetNpcND;
            if (obj != null)
            {
                if (obj.isSuccessful)
                {
                    GetMotionAnimator().SetMarkAsStopMoving(true);

                    // open ui
                    {
                        Actor actor = ActorsManager.GetInstance().GetActor(obj.npcId);
                        Utils.Log("TODO: Talking");
                        //if (actor.roleTalkConfig == null)
                        //{
                        //    Utils.Log("There is not talk config at npc " + obj.npcId);
                        //}
                        //else
                        //{
                        //    UIManager.GetInstance().OpenUI(UIManager.UIName.Talking, () =>
                        //    {
                        //        var talkingUI = UIManager.GetInstance().GetUI<Talking>(UIManager.UIName.Talking);
                        //        var talkingController = UIManager.GetInstance().GetUI<TalkingController>(UIManager.UIName.Talking);
                        //        talkingController.Initialize(talkingUI, actor.roleTalkConfig.storyboardName);
                        //    });
                        //}
                    }
                }
                else
                {
                    var operation = GetOperation().GenerateMeetNpcOperation(obj.npcId);
                    GetOperation().SetOperation(operation);
                }
            }
        }

        #endregion

        #region Break Wall

        private BreakWall _recentBreakWall = null;

        private BreakWallSkill _breakWallSkill = null;

        private void StopBreakWallSkill()
        {
            if (actor.o.HasInHandItem())
            {
                GetMotionAnimator().SetUpBodyAnimation(UpBodySM.Transition.StickInHands);
            }
            DeleteBreakWallSkill();
        }

        private void NewOperationHandler_BreakWall(NewOperationND data)
        {
            StopBreakWallSkill();
        }

        private void BreakWallClickedHUDHandler(NotificationData _data)
        {
            var data = _data as BreakWallHUDClickedND;
            if (data != null)
            {
                _recentBreakWall = data.breakWall;

                var operation = GetOperation().GenerateMovingOperation(_recentBreakWall.dynamicLink.a);
                GetOperation().SetOperation(operation);
            }
        }

        private void MoveToWaypointsEndHanlder_BreakWall(MoveToWaypointsEndND data)
        {
            if (_recentBreakWall != null &&
                _recentBreakWall.dynamicLink.IsAOrB(data.endWaypoint))
            {
                if (DataCenter.query.ActorHasKeyItem(actor.o.pd, _recentBreakWall.itemNeeded))
                {
                    if (actor.o.HasInHandItem())
                    {
                        var itemConfig = DataCenter.GetInstance().GetItemConfig(actor.o.pd.inHandItem.itemID);
                        var animName = Utils.StringToEnum<SkillSM.Transition>(itemConfig.anim);

                        GetMotionAnimator().StopShakeHead(false);
                        GetMotionAnimator().SetSkillState(animName);
                        GetMotionAnimator().SetUpBodyAnimation(UpBodySM.Transition.None);

                        _breakWallSkill = new BreakWallSkill(actor.o, _recentBreakWall, BreakWallSkillCompleteCB, BreakWallSkillItemBrokenCB);
                    }   
                }
                else
                {
                    GetMotionAnimator().StopShakeHead(true);
                    actor.o.TalkingBubble("break_wall_no_item");
                }
                    
            }
            _recentBreakWall = null;
        }

        private void BreakWallSkillCompleteCB(BreakWallSkill skill)
        {
            skill.breakWall.Collapse();
            StopBreakWallSkill();
            GetMotionAnimator().SetState(MotionAnimator.State.Idle);
        }

        private void BreakWallSkillItemBrokenCB(BreakWallSkill skill)
        {
            // Item is broken
            ModeratorUtils.DestroyItem(skill.actor.guid, skill.actor.pd.inHandItem.guid);
            StopBreakWallSkill();
            GetMotionAnimator().SetState(MotionAnimator.State.Idle);
        }

        private void DeleteBreakWallSkill()
        {
            _breakWallSkill = null;
        }

        private void UpateBreakWall()
        {
            if (_breakWallSkill != null)
            {
                _breakWallSkill.Update();
            }
        }

        #endregion

        #region Drop in hand item to scene when doing some animations

        private void UpdateDropItemToSceneWhenSomeAnimations()
        {
            if (GetMotionAnimator() != null && ActorsManager.GetInstance() != null && ActorsManager.GetInstance().GetHeroActor() != null)
            {
                // Can't keep in hand item when climbing 
                if (GetMotionAnimator().ContainsState(MotionAnimator.State.Climbing))
                {
                    var heroActor = ActorsManager.GetInstance().GetHeroActor();
                    var heroActorPD = DataCenter.GetInstance().playerData.GetSerializableMonoBehaviourPD<ActorPD>(heroActor.guid);
                    if (heroActorPD.inHandItem.IsEmpty() == false)
                    {
                        ModeratorUtils.DropItemToScene(heroActor.guid, heroActorPD.inHandItem.guid);
                    }
                }
            }
        }

        #endregion
    }
}
