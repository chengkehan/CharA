using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameScript.Cutscene;

namespace GameScript
{
    public class Query
    {
        public bool IsHeroRoleID(string roleID)
        {
            return roleID == AssetsManager.HERO_ROLE_ID;
        }

        public bool IsHeroRoleIdSimplified(string roleId)
        {
            return roleId == "0";
        }

        public bool IsWallBreaked(BreakWallPD breakWallPD)
        {
            if (breakWallPD == null)
            {
                return false;
            }
            else
            {
                return breakWallPD.health <= 0;
            }
        }

        public Vector3 AdjustSceneItemWorldPosition(Vector3 wPos)
        {
            wPos.y += DataCenter.define.SceneItemYOffset;
            return wPos;
        }

        public bool IsItemInActorPocket(ActorPD actorPD, string itemGUID, out Define.PocketType pocketType)
        {
            pocketType = Define.PocketType.Clothes_Left_Side;

            if (actorPD == null)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < actorPD.NumberPocketItems(); i++)
                {
                    if (actorPD.GetPocketItem(i).guid == itemGUID)
                    {
                        pocketType = (Define.PocketType)i;
                        return true;
                    }
                }
                return false;
            }
        }

        public bool ActorPocketIsFull(ActorPD actorPD)
        {
            if (actorPD == null)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < actorPD.NumberPocketItems(); i++)
                {
                    if (actorPD.GetPocketItem(i).IsEmpty())
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public Define.PocketType GetActorEmptyPocket(ActorPD actorPD)
        {
            Utils.Assert(ActorPocketIsFull(actorPD) == false);

            for (int i = 0; i < actorPD.NumberPocketItems(); i++)
            {
                if (actorPD.GetPocketItem(i).IsEmpty())
                {
                    return (Define.PocketType)i;
                }
            }

            throw new Exception("Unexpected");
        }

        public bool ItemCanBeInPocket(Define.ItemSpace itemSpace)
        {
            return itemSpace == Define.ItemSpace.One;
        }

        //public bool ItemCanBeInBackpack(Define.ItemSpace itemSpace)
        //{
        //    return ItemCanBeInPocket(itemSpace) || itemSpace == Define.ItemSpace.Four;
        //}

        public bool ItemCanBeInHandOnly(Define.ItemSpace itemSpace)
        {
            return itemSpace == Define.ItemSpace.Zero;
        }

        // for example, role has key to door, so that role can open this door.
        // items of a role should be mathcing all settings in one of pairs.
        public bool ActorHasKeyItem(ActorPD actorPD, PairsData pairsData)
        {
            return true;
            if (actorPD != null && pairsData != null)
            {
                for (int keyToDoorPairI = 0; keyToDoorPairI < pairsData.NumberPairs(); keyToDoorPairI++)
                {
                    var pair = pairsData.GetPair(keyToDoorPairI);

                    // checking items in pocket
                    {
                        int gotCount = 0;
                        for (int itemIDI = 0; itemIDI < pair.NumberItemIDs(); itemIDI++)
                        {
                            var itemID = pair.GetItemID(itemIDI);
                            for (int pocketI = 0; pocketI < actorPD.NumberPocketItems(); pocketI++)
                            {
                                var pocketItem = actorPD.GetPocketItem(pocketI);
                                if (pocketItem.IsEmpty() == false && pocketItem.itemID == itemID)
                                {
                                    ++gotCount;
                                }
                            }
                        }
                        if (gotCount == pair.NumberItemIDs())
                        {
                            return true;
                        }
                    }

                    // checking item in hand
                    {
                        for (int itemIDI = 0; itemIDI < pair.NumberItemIDs(); itemIDI++)
                        {
                            var itemID = pair.GetItemID(itemIDI);
                            if (actorPD.inHandItem.IsEmpty() == false && actorPD.inHandItem.itemID == itemID)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
