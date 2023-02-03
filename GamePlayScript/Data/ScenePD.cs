using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class ScenePD : SerializableMonoBehaviourPD
    {
        #region All SceneItemPD

        [SerializeField]
        private List<SceneItemPD> _allSceneItemPD = new List<SceneItemPD>();

        public int NumberSceneItemPD()
        {
            return _allSceneItemPD.Count;
        }

        public void RemoveSceneItemPD(string itemGUID)
        {
            for (int i = 0; i < NumberSceneItemPD(); i++)
            {
                if (GetSceneItemPD(i).guid == itemGUID)
                {
                    RemoveSceneItemPD(i);
                    break;
                }
            }
        }

        public void RemoveSceneItemPD(int index)
        {
            if (index >= 0 && index < _allSceneItemPD.Count)
            {
                _allSceneItemPD.RemoveAt(index);
            }
        }

        public SceneItemPD GetSceneItemPD(int index)
        {
            if (index < 0 || index >= _allSceneItemPD.Count)
            {
                return null;
            }
            else
            {
                return _allSceneItemPD[index];
            }
        }

        public SceneItemPD GetSceneItemPD(string itemGUID)
        {
            for (int i = 0; i < NumberSceneItemPD(); i++)
            {
                if (GetSceneItemPD(i).guid == itemGUID)
                {
                    return GetSceneItemPD(i);
                }
            }
            return null;
        }

        public bool ContainsSceneItemPD(string itemGUID)
        {
            for (int i = 0; i < NumberSceneItemPD(); i++)
            {
                if (GetSceneItemPD(i).guid == itemGUID)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddSceneItem(ItemPD itemPD, Vector3 wPos)
        {
            Utils.Assert(ContainsSceneItemPD(itemPD.guid) == false);

            var sceneItemPD = new SceneItemPD();
            sceneItemPD.Clone(itemPD);
            sceneItemPD.worldPosition = wPos;
            _allSceneItemPD.Add(sceneItemPD);
        }

        #endregion
    }
}
