using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class ItemRefresh : SerializableMonoBehaviour<ItemRefreshPD>
    {
        [SerializeField]
        private string _itemID = null;
        private string itemID
        {
            get
            {
                return _itemID;
            }
        }

        [SerializeField]
        [Min(0)]
        private int _maxRefreshTimes = 0;
        private int maxRefreshTimes
        {
            get
            {
                return _maxRefreshTimes;
            }
        }

#if UNITY_EDITOR
        public void RefreshItemNowTest()
        {
            var itemConfig = DataCenter.GetInstance().GetItemConfig(itemID);
            Scene.GetInstance().AddSceneItem(
                new ItemPD(guid, itemConfig), 
                DataCenter.query.AdjustSceneItemWorldPosition(transform.position)
            );
        }
#endif

        // using this.guid as refreshed new item's guid.
        // if item already existed in the world, we will not refresh again.
        public void RefreshItemNow()
        {
            if (pd.refreshTimes < maxRefreshTimes)
            {
                bool alreadyExistedInTheWorld = false;

                // Checking all actors
                {
                    var allActorPD = DataCenter.GetInstance().playerData.GetAllSerializableMonoBehaviourPD<ActorPD>();
                    for (int actorPDI = 0; actorPDI < allActorPD.Count; actorPDI++)
                    {
                        var actorPD = allActorPD[actorPDI];

                        // Checking pocket
                        for (int pocketI = 0; pocketI < actorPD.NumberPocketItems(); pocketI++)
                        {
                            var pocketItem = actorPD.GetPocketItem(pocketI);
                            if (pocketItem.IsEmpty() == false && pocketItem.guid == guid)
                            {
                                alreadyExistedInTheWorld = true;
                            }
                        }

                        // Checking hand
                        if (actorPD.inHandItem.IsEmpty() == false && actorPD.inHandItem.guid == guid)
                        {
                            alreadyExistedInTheWorld = true;
                        }
                    }
                }

                // Checking Scene
                {
                    var allScenePD = DataCenter.GetInstance().playerData.GetAllSerializableMonoBehaviourPD<ScenePD>();
                    for (int scenePDI = 0; scenePDI < allScenePD.Count; scenePDI++)
                    {
                        var scenePD = allScenePD[scenePDI];
                        for (int sceneItemPDI = 0; sceneItemPDI < scenePD.NumberSceneItemPD(); sceneItemPDI++)
                        {
                            var sceneItemPD = scenePD.GetSceneItemPD(sceneItemPDI);
                            if (sceneItemPD.guid == guid)
                            {
                                alreadyExistedInTheWorld = true;
                            }
                        }
                    }
                }

                if (alreadyExistedInTheWorld == false)
                {
                    var itemConfig = DataCenter.GetInstance().GetItemConfig(itemID);
                    Scene.GetInstance().AddSceneItem(
                        new ItemPD(guid, itemConfig), 
                        DataCenter.query.AdjustSceneItemWorldPosition(transform.position)
                    );
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            AssetsManager.GetInstance().UnloadSceneItem(guid);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color gizmosColor = Gizmos.color;
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 0.25f);
            }
            Gizmos.color = gizmosColor;
        }
#endif
    }
}
