using System.Collections;
using GameScript.Cutscene;
using UnityEngine;
using GameScript.UI.Talking;
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
            EventSystem.GetInstance().AddListener(EventID.PaperClicked, PaperClickedHandler);
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
            EventSystem.GetInstance().RemoveListener(EventID.PaperClicked, PaperClickedHandler);
        }

        private void NewOperationHandler(NotificationData _data)
        {
            var data = _data as NewOperationND;
            if (data != null && data.roleID == actor.o.GetId())
            {
                NewOperationHandler_BreakWall(data);
                NewOperationHandler_ReadPaper(data);
            }
        }

        private void MoveToWaypointsEndHanlder(NotificationData _data)
        {
            var data = _data as MoveToWaypointsEndND;
            if (data != null && data.roleID == actor.o.GetId())
            {
                MoveToWaypointsEndHanlder_BreakWall(data);
                MoveToWaypointsEndHanlder_SceneItemHUDClicked(data);
                MoveToWaypointsEndHandler_ReadPaper(data);
            }
        }

        #endregion

        #region Read Paper

        private Paper _pendingPaper = null;

        private Coroutine _openPaperDelay = null;

        private IEnumerator OpenPaperDelayCoroutine(Paper paper, float delaySeconds)
        {
            // Waiting for turning complete
            yield return new WaitForSeconds(delaySeconds);

            UIManager.GetInstance().OpenUI(UIManager.UIName.Paper, null);
        }

        private void StopOpenPaperDelay()
        {
            if (_openPaperDelay != null)
            {
                StopCoroutine(_openPaperDelay);
                _openPaperDelay = null;
            }
        }

        private void NewOperationHandler_ReadPaper(NewOperationND data)
        {
            StopOpenPaperDelay();
        }

        private void PaperClickedHandler(NotificationData _data)
        {
            var data = _data as PaperClickedND;
            if (data != null)
            {
                var paper = data.paper;
                var waypoint = GetWaypointPath().GetNearestMovingWaypointInBounds(paper.GetPosition(), paper.waypointsBounds.bounds, Matrix4x4.identity);
                if (waypoint != null)
                {
                    _pendingPaper = paper;

                    var operation = GetOperation().GenerateMovingOperation(waypoint);
                    GetOperation().SetOperation(operation);
                }
            }
        }

        private void MoveToWaypointsEndHandler_ReadPaper(MoveToWaypointsEndND data)
        {
            if (_pendingPaper != null)
            {
                var waypoint = GetWaypointPath().GetNearestMovingWaypointInBounds(_pendingPaper.GetPosition(), _pendingPaper.waypointsBounds.bounds, Matrix4x4.identity);
                if (waypoint == data.endWaypoint)
                {
                    GetMotionAnimator().StopShakeHead(false);

                    float delaySeconds = 0;
                    if (GetMotionAnimator().AtRightHandSide(_pendingPaper.GetPosition()))
                    {
                        delaySeconds = 1;
                        GetMotionAnimator().SetState(MotionAnimator.State.Turning, MotionAnimator.State.DirectionRight);
                    }
                    if (GetMotionAnimator().AtLeftHandSide(_pendingPaper.GetPosition()))
                    {
                        delaySeconds = 1;
                        GetMotionAnimator().SetState(MotionAnimator.State.Turning, MotionAnimator.State.DirectionLeft);
                    }

                    StopOpenPaperDelay();
                    _openPaperDelay = StartCoroutine(OpenPaperDelayCoroutine(_pendingPaper, delaySeconds));
                }
            }
            _pendingPaper = null;
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
                    var notification = new PickUpSceneItemND();
                    notification.roleID = actor.o.GetId();
                    notification.itemGUID = _sceneItemHUDClicked_ItemGUID;
                    EventSystem.GetInstance().Notify(EventID.PickUpSceneItem, notification);
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
                        if (actor.roleTalkConfig == null)
                        {
                            Utils.Log("There is not talk config at npc " + obj.npcId);
                        }
                        else
                        {
                            UIManager.GetInstance().OpenUI(UIManager.UIName.Talking, () =>
                            {
                                var talkingUI = UIManager.GetInstance().GetUI<Talking>(UIManager.UIName.Talking);
                                var talkingController = UIManager.GetInstance().GetUI<TalkingController>(UIManager.UIName.Talking);
                                talkingController.Initialize(talkingUI, actor.roleTalkConfig.storyboardName);
                            });
                        }
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
                GetMotionAnimator().SetUpBodyAnimation(MotionAnimator.UpBodyAnimation.StickInHands);
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
                        GetMotionAnimator().SetUpBodyAnimation(MotionAnimator.UpBodyAnimation.None);

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
            var notification = new DestroyItemND();
            notification.actorGUID = skill.actor.guid;
            notification.itemGUID = skill.actor.pd.inHandItem.guid;
            EventSystem.GetInstance().Notify(EventID.DestroyItem, notification);

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
    }
}
