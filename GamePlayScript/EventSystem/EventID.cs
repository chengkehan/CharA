using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public enum EventID
    {
        Undefined,
        ApplicationQuit,
        MeetNpc, // Hero meet a npc.(Hero only)
        BreakWallHUDClicked, // Hero wanna break wall.(Hero only)
        NewOperation, // Make a new command to control a role.
        MoveToWaypointsEnd, // Role arrive the last waypoint of path.
        TalkingBubble, // Role say something and a bubble showing on top of head.
        WannaOpenLockedDoor, // Role wanna open a door, but door is locked.
        AddSceneItem, // An item is added to scene.
        RemoveSceneItem,  // An item is deleted from scene.
        SceneItemHUDClicked, // Hero wanna pick up an item in scene.(Hero only)
        UGUI_EventSystemChanged,
        PickUpSceneItem, // Role is closed enough to item and ready to pick it up.
        DropItemToScene, // Role wanna drop an item to scene.
        DestroyItem, // Delete an item in role's hand or pocket.
        NoWayToPoint, 
        StopShakeHead,
        PickupableObjectClicked, // A pickupable object is clicked in scene. Hero wanna do some operation to it.(Hero only)
        SceneLoaded,
        TransferCardboardBoxItemToScene, 
        TransferCardboardBoxItemToActor,
        TransferPocketItemToCardboardBox,
        LoopTypeSoloComplete
    }
}
