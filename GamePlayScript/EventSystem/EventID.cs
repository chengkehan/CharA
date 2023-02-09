using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public enum EventID
    {
        Undefined,
        ApplicationQuit,
        MeetNpc,
        BreakWallHUDClicked,
        NewOperation,
        MoveToWaypointsEnd,
        TalkingBubble,
        WannaOpenLockedDoor,
        AddSceneItem,
        RemoveSceneItem, 
        SceneItemHUDClicked,
        UGUI_EventSystemChanged,
        PickUpSceneItem,
        DropItemToScene,
        DestroyItem,
        NoWayToPoint,
        StopShakeHead,
        PickupableObjectClicked,
        SceneLoaded,
        TransferCardboardBoxItemToScene,
        TransferCardboardBoxItemToActor
    }
}
