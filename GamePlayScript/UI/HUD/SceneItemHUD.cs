using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameScript.Cutscene;
using System;

namespace GameScript.UI.HUD
{
    public class SceneItemHUD : Project3DHUD
    {
        [SerializeField]
        private Button _button = null;
        private Button button
        {
            get
            {
                return _button;
            }
        }

        [SerializeField]
        private Image _image = null;
        private Image image
        {
            get
            {
                return _image;
            }
        }

        private string _itemGUID = null;
        public string itemGUID
        {
            private set
            {
                _itemGUID = value;
            }
            get
            {
                return _itemGUID;
            }
        }

        private SceneItemPD sceneItemPD = null;

        #region Neighbours

        private const float CONSTRAINT_DISTANCE = 90;

        private SceneItemHUD[] _neighbours = new SceneItemHUD[6];

        private List<SceneItemHUD> _neighboursSearchingTempList = new List<SceneItemHUD>();
        private List<SceneItemHUD> _neighboursSearchedTempList = new List<SceneItemHUD>();
        private List<SceneItemHUD> _neighboursPendingTempList = new List<SceneItemHUD>();

        public void ResetNeighbours()
        {
            for (int i = 0; i < _neighbours.Length; i++)
            {
                _neighbours[i] = null;
            }
        }
        private bool IsNeighboursFull()
        {
            for (int i = 0; i < _neighbours.Length; i++)
            {
                if (_neighbours[i] == null)
                {
                    return false;
                }
            }
            return true;
        }
        public SceneItemHUD GetUncrowdedNeighbour()
        {
            var searchingList = _neighboursSearchingTempList;
            var pendingList = _neighboursPendingTempList;
            var completeList = _neighboursSearchedTempList;
            pendingList.Clear();
            completeList.Clear();
            searchingList.Clear();
            searchingList.Add(this);

            while (true)
            {
                for (int i = 0; i < searchingList.Count; i++)
                {
                    completeList.Add(searchingList[i]);
                    if (searchingList[i].IsNeighboursFull() == false)
                    {
                        return searchingList[i];
                    }
                    else
                    {
                        for (int j = 0; j < searchingList[i]._neighbours.Length; j++)
                        {
                            if (completeList.Contains(searchingList[i]._neighbours[j]) == false)
                            {
                                pendingList.Add(searchingList[i]._neighbours[j]);
                            }
                        }
                    }
                }

                searchingList.Clear();
                searchingList.AddRange(pendingList);
                pendingList.Clear();
            }

            throw new Exception("Unexpected");
        }
        public void AddNeighbour(SceneItemHUD anotherOne)
        {
            if (anotherOne != null)
            {
                for (int i = 0; i < _neighbours.Length; i++)
                {
                    if (_neighbours[i] == null)
                    {
                        // link each other
                        _neighbours[i] = anotherOne;
                        anotherOne._neighbours[Repeat(i + 3)] = this;

                        int prevI = i - 1;
                        if (prevI < 0)
                        {
                            prevI = 5;
                        }
                        if (_neighbours[prevI] != null)
                        {
                            _neighbours[prevI]._neighbours[Repeat(prevI + 2)] = anotherOne;
                            anotherOne._neighbours[Repeat(i - 2)] = _neighbours[prevI];
                        }

                        int nextI = i + 1;
                        if (nextI > 5)
                        {
                            nextI = 0;
                        }
                        if (_neighbours[nextI] != null)
                        {
                            _neighbours[nextI]._neighbours[Repeat(nextI - 2)] = anotherOne;
                            anotherOne._neighbours[Repeat(i + 2)] = _neighbours[nextI];
                        }

                        // place neighbour
                        float pieRadian = Mathf.PI / 6.0f;
                        float radian = pieRadian + (i * 2) * pieRadian;
                        float dx = Mathf.Cos(radian) * CONSTRAINT_DISTANCE;
                        float dy = Mathf.Sin(radian) * CONSTRAINT_DISTANCE;
                        var placeNeighbour = GetAnchoredPoint();
                        placeNeighbour.x += dx;
                        placeNeighbour.y += dy;
                        anotherOne.SetAnchoredPoint(placeNeighbour);

                        return;
                    }
                }
            }
            throw new Exception("Unexpected");
        }

        private int Repeat(int v)
        {
            if (v >= 6)
            {
                return v - 6;
            }
            if (v < 0)
            {
                return 6 + v;
            }
            return v;
        }

        public bool CollideWith(SceneItemHUD anotherOne)
        {
            if (anotherOne == null || anotherOne == this || anotherOne.buttonVisible == false || buttonVisible == false)
            {
                return false;
            }
            else
            {
                return Vector2.Distance(GetAnchoredPoint(), anotherOne.GetAnchoredPoint()) < CONSTRAINT_DISTANCE;
            }
        }

        #endregion

        protected override void Start()
        {
            base.Start();

            button.onClick.AddListener(ButtonOnClickHandler);
        }

        private void ButtonOnClickHandler()
        {
            var notificationData = new SceneItemHUDClickedND();
            notificationData.itemGUID = sceneItemPD.guid;
            EventSystem.GetInstance().Notify(EventID.SceneItemHUDClicked, notificationData);
        }

        public void Set(string itemGUID)
        {
            image.enabled = false;
            this.itemGUID = itemGUID;

            var scenePD = Scene.GetInstance().pd;
            sceneItemPD = scenePD.GetSceneItemPD(itemGUID);
            if (sceneItemPD != null)
            {
                AssetsManager.GetInstance().LoadItemIcon(sceneItemPD.itemID, (obj) =>
                {
                    image.sprite = obj;
                    image.enabled = true;
                });
            }
        }

        public bool buttonVisible
        {
            get
            {
                return button != null && button.gameObject != null && button.gameObject.activeSelf;
            }
            set
            {
                if (button != null && button.gameObject != null)
                {
                    if (button.gameObject.activeSelf != value)
                    {
                        button.gameObject.SetActive(value);
                    }
                }
            }
        }

        protected override void UpdateVisibleByDistanceFromHero_Internal(Vector3 heroPosition)
        {
            if (sceneItemPD != null)
            {
                var sceneItemPosition = sceneItemPD.worldPosition;
                var inRange = Vector3.Distance(heroPosition, sceneItemPosition) < DataCenter.define.SceneItemVisibleRange;
                var atTheSameHorizontalLevel = Mathf.Abs(sceneItemPosition.y - heroPosition.y) < (DataCenter.define.SceneItemYOffset + 0.5f);
                buttonVisible = inRange && atTheSameHorizontalLevel && DataCenter.GetInstance().bloackboard.heroSoloAndMuteOthers == false;
            }            
        }

        protected override Vector3 Get3DPosition()
        {
            return sceneItemPD == null ? Vector3.zero : sceneItemPD.worldPosition;
        }
    }
}
