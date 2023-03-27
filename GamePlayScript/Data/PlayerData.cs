using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameScript.WaypointSystem;
using GameScript.Cutscene;

namespace GameScript
{
    // Shorthand for PD
    [Serializable]
    public class PlayerData
    {
        [SerializeField]
        private SerializableDictionary<string/*guid of Storyboard*/, StoryThreadPD> allStoryThreadPD = new SerializableDictionary<string, StoryThreadPD>();

        /*Auto Generate Config
        
         BreakWallPD | ActorPD | DoorPD | ScenePD | ItemRefreshPD | SequencePlayerPD
         DynamicLinkPD | PaperPD | DayNightPD | CardboardBoxPD | SoliloquyTriggerPD

         Auto Generate Config*/

        /*Auto Generate Fields*/

        private class PDObjects
        {
            public Type pdType = null;

            public Type monoType = null;

            public object collection = null;
        }

        private List<PDObjects> allPDObjects = new List<PDObjects>();

        public void OnLoad()
        {
            allPDObjects.Clear();
            /*Auto Generate Wrap*/

        }

        public void OnSave()
        {
            /*Auto Generate Saving*/

        }

        public T GetSerializableMonoBehaviourPD<T>(string guid)
            where T : SerializableMonoBehaviourPD, new()
        {
            var t = typeof(T);
            foreach (var pdObjects in allPDObjects)
            {
                if (pdObjects.pdType == t)
                {
                    var collection = pdObjects.collection as SerializableDictionary<string, T>;
                    if (collection.TryGetValue(guid, out var pd) == false)
                    {
                        pd = new T();
                        pd.guid.o = guid;
                        collection.Add(guid, pd);
                    }
                    return pd;
                }
            }

            Utils.Assert(false, "Unexpected type " + t.ToString());
            return default(T);
        }

        public SerializableDictionaryReadOnly<string, T> GetAllSerializableMonoBehaviourPD<T>()
            where T : SerializableMonoBehaviourPD
        {
            var t = typeof(T);
            foreach (var pdObjects in allPDObjects)
            {
                if (pdObjects.pdType == t)
                {
                    return pdObjects.collection as SerializableDictionaryReadOnly<string, T>;
                }
            }
            
            Utils.Assert(false, "Unexpected type " + t.ToString());
            return null;
        }

        public StoryThreadPD GetStoryThreadPD(string storyboardGuid)
        {
            if (allStoryThreadPD.TryGetValue(storyboardGuid, out var storyThreadPD) == false)
            {
                storyThreadPD = new StoryThreadPD(storyboardGuid);
                allStoryThreadPD.Add(storyboardGuid, storyThreadPD);
            }
            return storyThreadPD;
        }

        private void SaveSerializableMonoBehaviours<TPD, TMONO>(
            SerializableDictionary<string/*guid of pd*/, TPD> container
        )
            where TPD : SerializableMonoBehaviourPD, new()
            where TMONO : SerializableMonoBehaviour<TPD>
        {
            var allMonoInScene = GameObject.FindObjectsOfType<TMONO>(true);
            if (allMonoInScene != null)
            {
                foreach (var mono in allMonoInScene)
                {
                    if (mono != null && mono.pd != null)
                    {
                        mono.Save();
                        container[mono.guid] = mono.pd;
                    }
                }
            }
        }
    }
}