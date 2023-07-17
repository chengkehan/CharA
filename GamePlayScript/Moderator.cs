using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.Cutscene;

namespace GameScript
{
    // Some complex operations with several steps or need some validations.
    // We implement this complex operation and validatinos at here.
    // There is a high priority for events so that we can interrupt events when validation is not pass.
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
            EventSystem.GetInstance().AddListener(EventID.PickUpSceneItem, PickUpSceneItemHandler, EventSystem.ListenerPriority.High);
            EventSystem.GetInstance().AddListener(EventID.DropItemToScene, DropItemToSceneHandler, EventSystem.ListenerPriority.High);
            EventSystem.GetInstance().AddListener(EventID.DestroyItem, DestroyItemHandler, EventSystem.ListenerPriority.High);
            EventSystem.GetInstance().AddListener(EventID.TransferCardboardBoxItemToScene, TransferCardboardBoxItemToSceneHandler, EventSystem.ListenerPriority.High);
            EventSystem.GetInstance().AddListener(EventID.TransferCardboardBoxItemToActor, TransferCardboardBoxItemToActorHandler, EventSystem.ListenerPriority.High);
            EventSystem.GetInstance().AddListener(EventID.TransferPocketItemToCardboardBox, TransferPocketItemToCardboardBoxHandler, EventSystem.ListenerPriority.High);
            EventSystem.GetInstance().AddListener(EventID.AddSceneItem, AddSceneItemHandler, EventSystem.ListenerPriority.High);
            EventSystem.GetInstance().AddListener(EventID.RemoveSceneItem, RemoveSceneItemHandler, EventSystem.ListenerPriority.High);
        }

        private void RemoveListeners()
        {
            EventSystem.GetInstance().RemoveListener(EventID.PickUpSceneItem, PickUpSceneItemHandler);
            EventSystem.GetInstance().RemoveListener(EventID.DropItemToScene, DropItemToSceneHandler);
            EventSystem.GetInstance().RemoveListener(EventID.DestroyItem, DestroyItemHandler);
            EventSystem.GetInstance().RemoveListener(EventID.TransferCardboardBoxItemToScene, TransferCardboardBoxItemToSceneHandler);
            EventSystem.GetInstance().RemoveListener(EventID.TransferCardboardBoxItemToActor, TransferCardboardBoxItemToActorHandler);
            EventSystem.GetInstance().RemoveListener(EventID.TransferPocketItemToCardboardBox, TransferPocketItemToCardboardBoxHandler);
            EventSystem.GetInstance().RemoveListener(EventID.AddSceneItem, AddSceneItemHandler);
            EventSystem.GetInstance().RemoveListener(EventID.AddSceneItem, RemoveSceneItemHandler);
        }

        private void TransferPocketItemToCardboardBoxHandler(NotificationData _data)
        {
            var data = _data as TransferPocketItemToCardboardBoxND;
            if (data != null)
            {
                var actor = ActorsManager.GetInstance().GetActorByGUID(data.actorGUID);
                var actorPD = DataCenter.GetInstance().playerData.GetSerializableMonoBehaviourPD<ActorPD>(data.actorGUID);
                if (DataCenter.query.IsItemInActorPocket(actorPD, data.itemGUID, out var pocket))
                {
                    var cardboardBoxPD = DataCenter.GetInstance().playerData.GetSerializableMonoBehaviourPD<CardboardBoxPD>(data.cardboardBoxGUID);
                    if (cardboardBoxPD.ContainsItem(data.itemGUID) == false)
                    {
                        var itemPD = actorPD.GetPocketItem((int)pocket).Clone();
                        actor.DeletePocketItem(data.itemGUID);

                        cardboardBoxPD.AddItem(itemPD);
                    }
                    else
                    {
                        data.interrupted = true;
                    }
                }
                else
                {
                    data.interrupted = true;
                }
            }
            else
            {
                data.interrupted = true;
            }
        }

        private void TransferCardboardBoxItemToActorHandler(NotificationData _data)
        {
            var data = _data as TransferCardboardBoxItemToActorND;
            if (data != null)
            {
                var actorPD = DataCenter.GetInstance().playerData.GetSerializableMonoBehaviourPD<ActorPD>(data.actorGUID);
                if (DataCenter.query.ActorPocketIsFull(actorPD))
                {
                    Utils.Log("Pocket is full");
                    data.interrupted = true;
                }
                else
                {
                    var cardboardBoxPD = DataCenter.GetInstance().playerData.GetSerializableMonoBehaviourPD<CardboardBoxPD>(data.cardboardBoxGUID);
                    if (cardboardBoxPD.ContainsItem(data.itemGUID))
                    {
                        var itemPD = cardboardBoxPD.GetItemByGUID(data.itemGUID);
                        cardboardBoxPD.RemoveItem(data.itemGUID);

                        var emptyPocket = DataCenter.query.GetActorEmptyPocket(actorPD);
                        var actor = ActorsManager.GetInstance().GetActorByGUID(data.actorGUID);
                        actor.SetPocketItem(emptyPocket, itemPD);
                    }
                    else
                    {
                        data.interrupted = true;
                    }
                }
            }
            else
            {
                data.interrupted = true;
            }
        }

        private void TransferCardboardBoxItemToSceneHandler(NotificationData _data)
        {
            var data = _data as TransferCardboardBoxItemToSceneND;
            if (data != null)
            {
                // waiting for item gameobject removed from ui, then add to scene
                // we can't keep two item instances(assets) with same guid existed in the world at the same time.
                StartCoroutine(TransferCardboardBoxItemToSceneHandlerDelay(data));
            }
            else
            {
                data.interrupted = true;
            }
        }
        private IEnumerator TransferCardboardBoxItemToSceneHandlerDelay(TransferCardboardBoxItemToSceneND data)
        {
            yield return new WaitForSeconds(0.1f);

            var cardboardBoxPD = DataCenter.GetInstance().playerData.GetSerializableMonoBehaviourPD<CardboardBoxPD>(data.cardboardBoxGUID);
            if (cardboardBoxPD.ContainsItem(data.itemGUID))
            {
                var itemPD = cardboardBoxPD.GetItemByGUID(data.itemGUID);
                cardboardBoxPD.RemoveItem(data.itemGUID);
                ModeratorUtils.AddSceneItem(itemPD, data.dropPosition);
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
                    ModeratorUtils.AddSceneItem(
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
                        ModeratorUtils.AddSceneItem(
                            itemInPocket,  
                            DataCenter.query.AdjustSceneItemWorldPosition(actor.roleAnimation.GetMotionAnimator().GetPosition())
                        );
                    }
                    else
                    {
                        data.interrupted = true;
                        Utils.LogObservably("wtf");
                    }
                }
            }
            else
            {
                data.interrupted = true;
            }
        }

        private void PickUpSceneItemHandler(NotificationData _data)
        {
            var data = _data as PickUpSceneItemND;
            if (data != null)
            {
                var sceneItemPD = Scene.GetInstance().pd.GetSceneItemPD(data.itemGUID);
                var itemConfig = DataCenter.GetInstance().GetItemConfig(sceneItemPD.itemID);
                var actor = ActorsManager.GetInstance().GetActorByGUID(data.actorGUID);

                if (DataCenter.query.ItemCanBeInHandOnly((Define.ItemSpace)itemConfig.space))
                {
                    // delete from scene
                    ModeratorUtils.RemoveSceneItem(data.itemGUID);

                    // put down item already in hand
                    if (actor.HasInHandItem())
                    {
                        var inHandItem = actor.pd.inHandItem.Clone();

                        actor.DeleteInHandItem();

                        ModeratorUtils.AddSceneItem(
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
                            data.interrupted = true;
                            actor.TalkingBubble("pocket_full_when_pickup");
                        }
                        else
                        {
                            // delete from scene
                            ModeratorUtils.RemoveSceneItem(data.itemGUID);

                            var pocketType = DataCenter.query.GetActorEmptyPocket(actor.pd);

                            // pick up a new item
                            actor.SetPocketItem(pocketType, sceneItemPD);

                            actor.roleAnimation.GetMotionAnimator().StopShakeHead(false);
                        }
                    }
                    else
                    {
                        data.interrupted = true;
                        Utils.LogObservably("whf");
                    }
                }
            }
            else
            {
                data.interrupted = true;
            }
        }

        private void AddSceneItemHandler(NotificationData _data)
        {
            var data = _data as AddSceneItemND;
            if (data != null)
            {
                // Validate and intercept if it's invalid
                if (Scene.GetInstance().pd.ContainsSceneItemPD(data.itemPD.guid))
                {
                    Utils.LogError("Add Scene Item Failed. Duplicated guid.");
                    data.interrupted = true;
                }
            }
            else
            {
                data.interrupted = true;
            }
        }

        private void RemoveSceneItemHandler(NotificationData _data)
        {
            var data = _data as RemoveSceneItemND;
            if (data != null)
            {
                if (Scene.GetInstance().pd.ContainsSceneItemPD(data.itemGUID) == false)
                {
                    Utils.LogError("Remove Scene Item Failed. There is not a item that guid is " + data.itemGUID);
                    data.interrupted = true;
                }
            }
            else
            {
                data.interrupted = true;
            }
        }
    }
}
