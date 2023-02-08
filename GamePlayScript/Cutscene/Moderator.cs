using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class Moderator : MonoBehaviour
    {
        private void Awake()
        {
            AddListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            EventSystem.GetInstance().AddListener(EventID.PickUpSceneItem, PickUpSceneItemHandler);
            EventSystem.GetInstance().AddListener(EventID.DropItemToScene, DropItemToSceneHandler);
            EventSystem.GetInstance().AddListener(EventID.DestroyItem, DestroyItemHandler);
            EventSystem.GetInstance().AddListener(EventID.TransferCardboardBoxItemToScene, TransferCardboardBoxItemToSceneHandler);
        }

        private void RemoveListeners()
        {
            EventSystem.GetInstance().RemoveListener(EventID.PickUpSceneItem, PickUpSceneItemHandler);
            EventSystem.GetInstance().RemoveListener(EventID.DropItemToScene, DropItemToSceneHandler);
            EventSystem.GetInstance().RemoveListener(EventID.DestroyItem, DestroyItemHandler);
            EventSystem.GetInstance().RemoveListener(EventID.TransferCardboardBoxItemToScene, TransferCardboardBoxItemToSceneHandler);
        }

        private void TransferCardboardBoxItemToSceneHandler(NotificationData _data)
        {
            var data = _data as TransferCardboardBoxItemToSceneND;
            if (data != null)
            {
                // waiting for item gameobject removed from ui, then add to scene
                StartCoroutine(TransferCardboardBoxItemToSceneHandlerDelay(data));
            }
        }
        private IEnumerator TransferCardboardBoxItemToSceneHandlerDelay(TransferCardboardBoxItemToSceneND data)
        {
            yield return new WaitForSeconds(0.5f);

            var cardboardBoxPD = DataCenter.GetInstance().playerData.GetSerializableMonoBehaviourPD<CardboardBoxPD>(data.cardboardBoxGUID);
            if (cardboardBoxPD.ContainsItem(data.itemGUID))
            {
                var itemPD = cardboardBoxPD.GetItemByGUID(data.itemGUID);
                cardboardBoxPD.RemoveItem(data.itemGUID);
                Scene.GetInstance().AddSceneItem(itemPD, data.dropPosition);
            }
        }

        private void DestroyItemHandler(NotificationData _data)
        {
            var data = _data as DestroyItemND;
            if (data != null)
            {
                var actor = ActorsManager.GetInstance().GetActorByGUID(data.actorGUID);
                var actorPD = actor.pd;

                // item in hand
                if (actorPD.inHandItem.IsEmpty() == false && actorPD.inHandItem.guid == data.itemGUID)
                {
                    actor.DeleteInHandItem();
                }
                // item in pocket
                else
                {
                    for (int pocketI = 0; pocketI < actorPD.NumberPocketItems(); pocketI++)
                    {
                        var pocketItem = actorPD.GetPocketItem(pocketI);
                        if (pocketItem.IsEmpty() == false && pocketItem.guid == data.itemGUID)
                        {
                            actor.DeletePocketItem(data.itemGUID);
                            break;
                        }
                    }
                }
            }
        }

        private void DropItemToSceneHandler(NotificationData _data)
        {
            var data = _data as DropItemToSceneND;
            if (data != null)
            {
                var actor = ActorsManager.GetInstance().GetActorByGUID(data.actorGUID);
                var actorPD = actor.pd;

                if (actorPD.inHandItem.IsEmpty() == false && actorPD.inHandItem.guid == data.itemGUID)
                {
                    // drop item in hand
                    var inHandItem = actorPD.inHandItem.Clone();

                    actor.DeleteInHandItem();
                    Scene.GetInstance().AddSceneItem(
                        inHandItem, 
                        DataCenter.query.AdjustSceneItemWorldPosition(actor.roleAnimation.GetMotionAnimator().GetPosition())
                    );
                }
                else
                {
                    if (DataCenter.query.IsItemInActorPocket(actorPD, data.itemGUID, out var pocketType))
                    {
                        // drop item in pocket
                        var itemInPocket = actorPD.GetPocketItem((int)pocketType).Clone();

                        actor.DeletePocketItem(itemInPocket.guid);
                        Scene.GetInstance().AddSceneItem(
                            itemInPocket,  
                            DataCenter.query.AdjustSceneItemWorldPosition(actor.roleAnimation.GetMotionAnimator().GetPosition())
                        );
                    }
                    else
                    {
                        Utils.Log("wtf");
                    }
                }
            }
        }

        private void PickUpSceneItemHandler(NotificationData _data)
        {
            var data = _data as PickUpSceneItemND;
            if (data != null)
            {
                var sceneItemPD = Scene.GetInstance().pd.GetSceneItemPD(data.itemGUID);
                var itemConfig = DataCenter.GetInstance().GetItemConfig(sceneItemPD.itemID);
                var actor = ActorsManager.GetInstance().GetActor(data.roleID);

                if (DataCenter.query.ItemCanBeInHandOnly((Define.ItemSpace)itemConfig.space))
                {
                    // delete from scene
                    Scene.GetInstance().RemoveSceneItem(data.itemGUID);

                    // put down item already in hand
                    if (actor.HasInHandItem())
                    {
                        var inHandItem = actor.pd.inHandItem.Clone();

                        actor.DeleteInHandItem();

                        Scene.GetInstance().AddSceneItem(
                            inHandItem,  
                            DataCenter.query.AdjustSceneItemWorldPosition(actor.roleAnimation.GetMotionAnimator().GetPosition())
                        );
                    }
                    // pick up a new item
                    actor.SetInHandItem(sceneItemPD);

                    actor.roleAnimation.GetMotionAnimator().StopShakeHead(false);
                }
                else
                {
                    if (DataCenter.query.ItemCanBeInPocket((Define.ItemSpace)itemConfig.space))
                    {
                        if (DataCenter.query.ActorPocketIsFull(actor.pd))
                        {
                            actor.TalkingBubble("pocket_full_when_pickup");
                        }
                        else
                        {
                            // delete from scene
                            Scene.GetInstance().RemoveSceneItem(data.itemGUID);

                            var pocketType = DataCenter.query.GetActorEmptyPocket(actor.pd);

                            // pick up a new item
                            actor.SetPocketItem(pocketType, sceneItemPD);

                            actor.roleAnimation.GetMotionAnimator().StopShakeHead(false);
                        }
                    }
                    else
                    {
                        Utils.Log("whf");
                    }
                }
            }
        }
    }
}
