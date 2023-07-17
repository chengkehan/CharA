using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class ModeratorUtils
    {
        public static void PickUpSceneItem(string actorGUID, string itemGUID)
        {
            var notification = new PickUpSceneItemND();
            notification.actorGUID = actorGUID;
            notification.itemGUID = itemGUID;
            EventSystem.GetInstance().Notify(EventID.PickUpSceneItem, notification);
        }

        public static void DropItemToScene(string actorGUID, string itemGUID)
        {
            var notificationData = new DropItemToSceneND();
            notificationData.actorGUID = actorGUID;
            notificationData.itemGUID = itemGUID;
            EventSystem.GetInstance().Notify(EventID.DropItemToScene, notificationData);
        }

        public static void DestroyItem(string actorGUID, string itemGUID)
        {
            var notification = new DestroyItemND();
            notification.actorGUID = actorGUID;
            notification.itemGUID = itemGUID;
            EventSystem.GetInstance().Notify(EventID.DestroyItem, notification);
        }

        public static void TransferCardboardBoxItemToScene(string cardboardBoxGUID, string itemGUID, Vector3 dropPosition)
        {
            var notificationData = new TransferCardboardBoxItemToSceneND();
            notificationData.cardboardBoxGUID = cardboardBoxGUID;
            notificationData.itemGUID = itemGUID;
            notificationData.dropPosition = dropPosition;
            EventSystem.GetInstance().Notify(EventID.TransferCardboardBoxItemToScene, notificationData);
        }

        public static void TransferCardboardBoxItemToActor(string cardboardBoxGUID, string itemGUID, string actorGUID)
        {
            var notificationData = new TransferCardboardBoxItemToActorND();
            notificationData.cardboardBoxGUID = cardboardBoxGUID;
            notificationData.itemGUID = itemGUID;
            notificationData.actorGUID = actorGUID;
            EventSystem.GetInstance().Notify(EventID.TransferCardboardBoxItemToActor, notificationData);
        }

        public static void TransferPocketItemToCardboardBox(string actorGUID, string itemGUID, string cardboardBoxGUID)
        {
            var notificationData = new TransferPocketItemToCardboardBoxND();
            notificationData.actorGUID = actorGUID;
            notificationData.itemGUID = itemGUID;
            notificationData.cardboardBoxGUID = cardboardBoxGUID;
            EventSystem.GetInstance().Notify(EventID.TransferPocketItemToCardboardBox, notificationData);
        }

        public static void AddSceneItem(ItemPD itemPD, Vector3 wPos)
        {
            var notificationData = new AddSceneItemND();
            notificationData.itemPD = itemPD;
            notificationData.worldSpacePosition = wPos;
            EventSystem.GetInstance().Notify(EventID.AddSceneItem, notificationData);
        }

        public static void RemoveSceneItem(string itemGUID)
        {
            var notificationData = new RemoveSceneItemND();
            notificationData.itemGUID = itemGUID;
            EventSystem.GetInstance().Notify(EventID.RemoveSceneItem, notificationData);
        }
    }
}
