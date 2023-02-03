using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class ActorPD : SerializableMonoBehaviourIPD
    {
        [SerializeField]
        private Vector3 _position = Vector3.zero;
        public Vector3 position
        {
            set
            {
                _position = value;
            }
            get
            {
                return _position;
            }
        }

        [SerializeField]
        private ItemPD _inHandItem = new ItemPD();
        public ItemPD inHandItem
        {
            get
            {
                return _inHandItem;
            }
        }

        #region Pocket Items

        [SerializeField]
        private ItemPD[] _pocketItems = new ItemPD[4] { new ItemPD(), new ItemPD(), new ItemPD(), new ItemPD() };

        public ItemPD GetPocketItem(int index)
        {
            if (index < 0 || index >= _pocketItems.Length)
            {
                return null;
            }
            return _pocketItems[index];
        }

        public int NumberPocketItems()
        {
            return _pocketItems.Length;
        }

        #endregion
    }
}
