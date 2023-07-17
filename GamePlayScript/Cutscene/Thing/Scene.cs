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

        private void AddSceneItem_Internal(ItemPD itemPD, Vector3 wPos, bool addPD)
        {
            var sceneItemGo = AssetsManager.GetInstance().LoadSceneItem(itemPD.guid, itemPD.itemID);
            sceneItemGo.transform.parent = transform;
            sceneItemGo.transform.position = wPos;
            sceneItemGo.transform.eulerAngles = new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));

            if (addPD)
            {
                pd.AddSceneItem(itemPD, wPos);
            }
        }

        protected override void InitializeOnAwake()
        {
            base.InitializeOnAwake();
            s_instance = this;

            AddListeners();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            s_instance = null;

            RemoveListeners();
        }

        #region Listeners

        private void AddListeners()
        {
            EventSystem.GetInstance().AddListener(EventID.AddSceneItem, AddSceneItemHandler);
            EventSystem.GetInstance().AddListener(EventID.RemoveSceneItem, RemoveSceneItemHandler);
        }

        private void RemoveListeners()
        {
            EventSystem.GetInstance().RemoveListener(EventID.AddSceneItem, AddSceneItemHandler);
            EventSystem.GetInstance().RemoveListener(EventID.RemoveSceneItem, RemoveSceneItemHandler);
        }

        private void AddSceneItemHandler(NotificationData _data)
        {
            var data = _data as AddSceneItemND;
            if (data != null)
            {
                AddSceneItem_Internal(data.itemPD, data.worldSpacePosition, true);
            }
        }

        private void RemoveSceneItemHandler(NotificationData _data)
        {
            var data = _data as RemoveSceneItemND;
            if (data != null)
            {
                AssetsManager.GetInstance().UnloadSceneItem(data.itemGUID);
                pd.RemoveSceneItemPD(data.itemGUID);
            }
        }

        #endregion

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
    }
}
