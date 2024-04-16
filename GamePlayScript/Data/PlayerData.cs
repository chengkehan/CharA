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
        
         BreakWall | Actor | Door | Scene | ItemRefresh | SequencePlayer
         DynamicLink | Paper | DayNight | CardboardBox | Soliloquy
         BoundsTrigger | RoleSpawn | StoryboardOneSelector | StoryboardLoopSelector
         Dialogue | ItemTempStorage

         Auto Generate Config*/

        /*Auto Generate Fields*/
[SerializeField] private SerializableDictionary<string, BreakWallPD> allBreakWallPD = new SerializableDictionary<string, BreakWallPD>();
[SerializeField] private SerializableDictionary<string, ActorPD> allActorPD = new SerializableDictionary<string, ActorPD>();
[SerializeField] private SerializableDictionary<string, DoorPD> allDoorPD = new SerializableDictionary<string, DoorPD>();
[SerializeField] private SerializableDictionary<string, ScenePD> allScenePD = new SerializableDictionary<string, ScenePD>();
[SerializeField] private SerializableDictionary<string, ItemRefreshPD> allItemRefreshPD = new SerializableDictionary<string, ItemRefreshPD>();
[SerializeField] private SerializableDictionary<string, SequencePlayerPD> allSequencePlayerPD = new SerializableDictionary<string, SequencePlayerPD>();
[SerializeField] private SerializableDictionary<string, DynamicLinkPD> allDynamicLinkPD = new SerializableDictionary<string, DynamicLinkPD>();
[SerializeField] private SerializableDictionary<string, PaperPD> allPaperPD = new SerializableDictionary<string, PaperPD>();
[SerializeField] private SerializableDictionary<string, DayNightPD> allDayNightPD = new SerializableDictionary<string, DayNightPD>();
[SerializeField] private SerializableDictionary<string, CardboardBoxPD> allCardboardBoxPD = new SerializableDictionary<string, CardboardBoxPD>();
[SerializeField] private SerializableDictionary<string, SoliloquyPD> allSoliloquyPD = new SerializableDictionary<string, SoliloquyPD>();
[SerializeField] private SerializableDictionary<string, BoundsTriggerPD> allBoundsTriggerPD = new SerializableDictionary<string, BoundsTriggerPD>();
[SerializeField] private SerializableDictionary<string, RoleSpawnPD> allRoleSpawnPD = new SerializableDictionary<string, RoleSpawnPD>();
[SerializeField] private SerializableDictionary<string, StoryboardOneSelectorPD> allStoryboardOneSelectorPD = new SerializableDictionary<string, StoryboardOneSelectorPD>();
[SerializeField] private SerializableDictionary<string, StoryboardLoopSelectorPD> allStoryboardLoopSelectorPD = new SerializableDictionary<string, StoryboardLoopSelectorPD>();
[SerializeField] private SerializableDictionary<string, DialoguePD> allDialoguePD = new SerializableDictionary<string, DialoguePD>();
[SerializeField] private SerializableDictionary<string, ItemTempStoragePD> allItemTempStoragePD = new SerializableDictionary<string, ItemTempStoragePD>();
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
allPDObjects.Add(new PDObjects() { pdType = typeof(SoliloquyPD), monoType = typeof(Soliloquy), collection = allSoliloquyPD });
allPDObjects.Add(new PDObjects() { pdType = typeof(BoundsTriggerPD), monoType = typeof(BoundsTrigger), collection = allBoundsTriggerPD });
allPDObjects.Add(new PDObjects() { pdType = typeof(RoleSpawnPD), monoType = typeof(RoleSpawn), collection = allRoleSpawnPD });
allPDObjects.Add(new PDObjects() { pdType = typeof(StoryboardOneSelectorPD), monoType = typeof(StoryboardOneSelector), collection = allStoryboardOneSelectorPD });
allPDObjects.Add(new PDObjects() { pdType = typeof(StoryboardLoopSelectorPD), monoType = typeof(StoryboardLoopSelector), collection = allStoryboardLoopSelectorPD });
allPDObjects.Add(new PDObjects() { pdType = typeof(DialoguePD), monoType = typeof(Dialogue), collection = allDialoguePD });
allPDObjects.Add(new PDObjects() { pdType = typeof(ItemTempStoragePD), monoType = typeof(ItemTempStorage), collection = allItemTempStoragePD });
            /*Auto Generate Wrap*/
        }

        public void OnSave()
        {
            /*Auto Generate Saving*/
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
SaveSerializableMonoBehaviours<SoliloquyPD, Soliloquy>(allSoliloquyPD);
SaveSerializableMonoBehaviours<BoundsTriggerPD, BoundsTrigger>(allBoundsTriggerPD);
SaveSerializableMonoBehaviours<RoleSpawnPD, RoleSpawn>(allRoleSpawnPD);
SaveSerializableMonoBehaviours<StoryboardOneSelectorPD, StoryboardOneSelector>(allStoryboardOneSelectorPD);
SaveSerializableMonoBehaviours<StoryboardLoopSelectorPD, StoryboardLoopSelector>(allStoryboardLoopSelectorPD);
SaveSerializableMonoBehaviours<DialoguePD, Dialogue>(allDialoguePD);
SaveSerializableMonoBehaviours<ItemTempStoragePD, ItemTempStorage>(allItemTempStoragePD);
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
