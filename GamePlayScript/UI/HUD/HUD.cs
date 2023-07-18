using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameScript.WaypointSystem;
using GameScript.Cutscene;

namespace GameScript.UI.HUD
{
    public class HUD : MonoBehaviour
    {
        [SerializeField]
        private DoorHUD _doorHUDPrefab = null;
        private DoorHUD doorHUDPrefab
        {
            get
            {
                return _doorHUDPrefab;
            }
        }

        [SerializeField]
        private BreakWallHUD _breakWallHUDPrefab = null;
        private BreakWallHUD breakWallHUDPrefab
        {
            get
            {
                return _breakWallHUDPrefab;
            }
        }

        [SerializeField]
        private Transform _breakWallHUDContainer = null;
        private Transform breakWallHUDContainer
        {
            get
            {
                return _breakWallHUDContainer;
            }
        }

        [SerializeField]
        private TalkingBubblesHUD _talkingBubbleHUDPrefab = null;
        private TalkingBubblesHUD talkingBubbleHUDPrefab
        {
            get
            {
                return _talkingBubbleHUDPrefab;
            }
        }

        [SerializeField]
        private Transform _talkingBubbleHUDContainer = null;
        private Transform talkingBubbleHUDContainer
        {
            get
            {
                return _talkingBubbleHUDContainer;
            }
        }

        [SerializeField]
        private SceneItemHUD _sceneItemHUDPrefab = null;
        private SceneItemHUD sceneItemHUDPrefab
        {
            get
            {
                return _sceneItemHUDPrefab;
            }
        }

        [SerializeField]
        private Transform _sceneItemHUDContainer = null;
        private Transform sceneItemHUDContainer
        {
            get
            {
                return _sceneItemHUDContainer;
            }
        }

        private List<BreakWallHUD> allBreakWallHUD = new List<BreakWallHUD>();

        private List<TalkingBubblesHUD> allTalkingBubbleHUD = new List<TalkingBubblesHUD>();

        private List<SceneItemHUD> allSceneItemHUD = new List<SceneItemHUD>();

        private void Awake()
        {
            AddListeners();
        }

        private void Start()
        {
            InitializeAllBreakWallHUD();
        }

        private void LateUpdate()
        {
            LateUpdate_SceneItemHUDBounceEachOther();
        }

        #region scene item hud bounce each other

        private void LateUpdate_SceneItemHUDBounceEachOther()
        {
            for (int i = 0; i < allSceneItemHUD.Count; i++)
            {
                allSceneItemHUD[i].ResetNeighbours();
            }

            for (int i = 0; i < allSceneItemHUD.Count; i++)
            {
                var a = allSceneItemHUD[i];
                for (int j = 0; j < i; j++)
                {
                    var b = allSceneItemHUD[j];
                    if (a != b && a.CollideWith(b))
                    {
                        var bb = b.GetUncrowdedNeighbour();
                        bb.AddNeighbour(a);
                        break;
                    }
                }
            }
        }

        #endregion

        private void InitializeAllBreakWallHUD()
        {
            var allBreakWall = FindObjectsOfType<BreakWall>(true);
            foreach (var breakWall in allBreakWall)
            {
                var breakWallHUDGo = Utils.InstantiateUIPrefab(breakWallHUDPrefab.gameObject, breakWallHUDContainer);
                breakWallHUDGo.SetActive(true);
                var breakWallHUD = breakWallHUDGo.GetComponent<BreakWallHUD>();
                breakWallHUD.breakWall = breakWall;
                breakWallHUD.UpdatePositionAndVisible();
                allBreakWallHUD.Add(breakWallHUD);
            }
        }

        private void AddListeners()
        {
            EventSystem.GetInstance().AddListener(EventID.TalkingBubble, TalkingBubbleHandler, gameObject);
            EventSystem.GetInstance().AddListener(EventID.AddSceneItem, AddSceneItemHandler, gameObject);
            EventSystem.GetInstance().AddListener(EventID.RemoveSceneItem, RemoveSceneItemHandler, gameObject);
        }

        private void RemoveSceneItemHandler(NotificationData _data)
        {
            var data = _data as RemoveSceneItemND;
            if (data != null)
            {
                for (int i = 0; i < allSceneItemHUD.Count; i++)
                {
                    if (allSceneItemHUD[i].itemGUID == data.itemGUID)
                    {
                        Utils.Destroy(allSceneItemHUD[i].gameObject);
                        allSceneItemHUD.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void AddSceneItemHandler(NotificationData _data)
        {
            var data = _data as AddSceneItemND;
            if (data != null)
            {
                var sceneItemHUDGo = Utils.InstantiateUIPrefab(sceneItemHUDPrefab.gameObject, sceneItemHUDContainer);
                sceneItemHUDGo.SetActive(true);
                var sceneItemHUD = sceneItemHUDGo.GetComponent<SceneItemHUD>();
                sceneItemHUD.Set(data.itemPD.guid);
                sceneItemHUD.UpdatePositionAndVisible();
                allSceneItemHUD.Add(sceneItemHUD);
            }
        }

        private void TalkingBubbleHandler(NotificationData _data)
        {
            var data = _data as TalkingBubbleND;
            if (data != null)
            {
                bool updated = false;
                for (int talkingBubbleI = 0; talkingBubbleI < allTalkingBubbleHUD.Count; talkingBubbleI++)
                {
                    if (allTalkingBubbleHUD[talkingBubbleI] == null)
                    {
                        allTalkingBubbleHUD.RemoveAt(talkingBubbleI);
                        talkingBubbleI--;
                    }
                    else
                    {
                        if (allTalkingBubbleHUD[talkingBubbleI].actorGUID == data.actorGUID)
                        {
                            allTalkingBubbleHUD[talkingBubbleI].Show(data.actorGUID, data.talkingText, data.duration);
                            updated = true;
                            break;
                        }
                    }
                }

                if (updated == false)
                {
                    var talkingBubbleHUDGo = Utils.InstantiateUIPrefab(talkingBubbleHUDPrefab.gameObject, talkingBubbleHUDContainer);
                    talkingBubbleHUDGo.SetActive(true);
                    var talkingBubbleHUD = talkingBubbleHUDGo.GetComponent<TalkingBubblesHUD>();
                    talkingBubbleHUD.Show(data.actorGUID, data.talkingText, data.duration);
                    allTalkingBubbleHUD.Add(talkingBubbleHUD);
                }
            }
        }
    }
}
