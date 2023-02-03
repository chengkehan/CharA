using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class Scene : SerializableMonoBehaviour<ScenePD>
    {
        private static Scene s_instance = null;
        public static Scene GetInstance()
        {
            return s_instance;
        }

        public void RemoveSceneItem(string itemGUID)
        {
            AssetsManager.GetInstance().UnloadSceneItem(itemGUID);
            pd.RemoveSceneItemPD(itemGUID);
            SendRemoveSceneItemEvent(itemGUID);
        }

        public void AddSceneItem(ItemPD itemPD, Vector3 wPos)
        {
            AddSceneItem_Internal(itemPD, wPos, true);
        }

        private void AddSceneItem_Internal(ItemPD itemPD, Vector3 wPos, bool addPD)
        {
            AssetsManager.GetInstance().LoadSceneItem(itemPD.guid, itemPD.itemID, (go) =>
            {
                go.transform.parent = transform;
                go.transform.position = wPos;
                go.transform.eulerAngles = new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));

                if (addPD)
                {
                    pd.AddSceneItem(itemPD, wPos);
                }

                SendAddSceneItemEvent(itemPD.guid);
            });
        }

        protected override void InitializeOnAwake()
        {
            base.InitializeOnAwake();
            s_instance = this;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            s_instance = null;
        }

        #region Spawn items and roles on startup

        // execute only once
        private bool _alreadySpawned = false;

        public void SpawnItemsAndRoleOnStartup()
        {
            if (_alreadySpawned == false)
            {
                _alreadySpawned = true;

                {
                    for (int sceneItemPDI = 0; sceneItemPDI < pd.NumberSceneItemPD(); sceneItemPDI++)
                    {
                        var sceneItemPD = pd.GetSceneItemPD(sceneItemPDI);
                        AddSceneItem_Internal(sceneItemPD, sceneItemPD.worldPosition, false);
                    }
                }

                {
                    var allItemRefresh = FindObjectsOfType<ItemRefresh>();
                    if (allItemRefresh != null)
                    {
                        foreach (var itemRefresh in allItemRefresh)
                        {
                            if (itemRefresh != null)
                            {
                                itemRefresh.RefreshItemNow();
                            }
                        }
                    }
                }

                {
                    var allRoleSpawn = FindObjectsOfType<RoleSpawn>();
                    if (allRoleSpawn != null)
                    {
                        foreach (var roleSpawn in allRoleSpawn)
                        {
                            if (roleSpawn != null)
                            {
                                roleSpawn.Spawn();
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private void SendAddSceneItemEvent(string itemGUID)
        {
            var notificationData = new AddSceneItemND();
            notificationData.itemGUID = itemGUID;
            EventSystem.GetInstance().Notify(EventID.AddSceneItem, notificationData);
        }

        private void SendRemoveSceneItemEvent(string itemGUID)
        {
            var notificationData = new RemoveSceneItemND();
            notificationData.itemGUID = itemGUID;
            EventSystem.GetInstance().Notify(EventID.RemoveSceneItem, notificationData);
        }
    }
}
