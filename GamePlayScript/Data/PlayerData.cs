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

        [SerializeField]
        private SerializableDictionary<string/*guid of pd*/, BreakWallPD> allBreakWallPD = new SerializableDictionary<string, BreakWallPD>();

        [SerializeField]
        private SerializableDictionary<string/*guid*/, ActorPD> allActorPD = new SerializableDictionary<string, ActorPD>();

        [SerializeField]
        private SerializableDictionary<string, DoorPD> allDoorPD = new SerializableDictionary<string, DoorPD>();

        [SerializeField]
        private SerializableDictionary<string, ScenePD> allScenePD = new SerializableDictionary<string, ScenePD>();

        [SerializeField]
        private SerializableDictionary<string, ItemRefreshPD> allItemRefreshPD = new SerializableDictionary<string, ItemRefreshPD>();

        [SerializeField]
        private SerializableDictionary<string, SequencePlayerPD> allSequencePlayerPD = new SerializableDictionary<string, SequencePlayerPD>();

        [SerializeField]
        private SerializableDictionary<string, DynamicLinkPD> allDynamicLinkPD = new SerializableDictionary<string, DynamicLinkPD>();

        [SerializeField]
        private SerializableDictionary<string, PaperPD> allPaperPD = new SerializableDictionary<string, PaperPD>();

        [SerializeField]
        private SerializableDictionary<string, DayNightPD> allDayNightPD = new SerializableDictionary<string, DayNightPD>();

        [SerializeField]
        private SerializableDictionary<string, CardboardBoxPD> allCardboardBoxPD = new SerializableDictionary<string, CardboardBoxPD>();

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
            allPDObjects.Add(new PDObjects() { pdType = typeof(BreakWallPD), monoType = typeof(BreakWall), collection = allBreakWallPD });
            allPDObjects.Add(new PDObjects() { pdType = typeof(ActorPD), monoType = typeof(Actor), collection = allActorPD });
            allPDObjects.Add(new PDObjects() { pdType = typeof(DoorPD), monoType = typeof(Door), collection = allDoorPD });
            allPDObjects.Add(new PDObjects() { pdType = typeof(ScenePD), monoType = typeof(Scene), collection = allScenePD });
            allPDObjects.Add(new PDObjects() { pdType = typeof(ItemRefreshPD), monoType = typeof(ItemRefresh), collection = allItemRefreshPD });
            allPDObjects.Add(new PDObjects() { pdType = typeof(SequencePlayerPD), monoType = typeof(SequencePlayer), collection = allSequencePlayerPD });
            allPDObjects.Add(new PDObjects() { pdType = typeof(DynamicLinkPD), monoType = typeof(DynamicLink), collection = allDynamicLinkPD });
            allPDObjects.Add(new PDObjects() { pdType = typeof(PaperPD), monoType = typeof(Paper), collection = allPaperPD });
            allPDObjects.Add(new PDObjects() { pdType = typeof(DayNightPD), monoType = typeof(DayNight), collection = allDayNightPD });
            allPDObjects.Add(new PDObjects() { pdType = typeof(CardboardBoxPD), monoType = typeof(CardboardBox), collection = allCardboardBoxPD });
        }

        public void OnSave()
        {
            SaveSerializableMonoBehaviours<BreakWallPD, BreakWall>(allBreakWallPD);
            SaveSerializableMonoBehaviours<ActorPD, Actor>(allActorPD);
            SaveSerializableMonoBehaviours<DoorPD, Door>(allDoorPD);
            SaveSerializableMonoBehaviours<ScenePD, Scene>(allScenePD);
            SaveSerializableMonoBehaviours<ItemRefreshPD, ItemRefresh>(allItemRefreshPD);
            SaveSerializableMonoBehaviours<SequencePlayerPD, SequencePlayer>(allSequencePlayerPD);
            SaveSerializableMonoBehaviours<DynamicLinkPD, DynamicLink>(allDynamicLinkPD);
            SaveSerializableMonoBehaviours<PaperPD, Paper>(allPaperPD);
            SaveSerializableMonoBehaviours<DayNightPD, DayNight>(allDayNightPD);
            SaveSerializableMonoBehaviours<CardboardBoxPD, CardboardBox>(allCardboardBoxPD);
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