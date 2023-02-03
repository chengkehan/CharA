using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class ItemPD
    {
        [SerializeField]
        private string _guid = null;
        public string guid
        {
            set
            {
                _guid = value;
            }
            get
            {
                return _guid;
            }
        }

        [SerializeField]
        private string _itemID = null;
        public string itemID
        {
            set
            {
                _itemID = value;
            }
            get
            {
                return _itemID;
            }
        }

        [SerializeField]
        private float _durability = 0;
        public float durability
        {
            set
            {
                _durability = value;
            }
            get
            {
                return _durability;
            }
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(itemID);
        }

        public void SetEmpty()
        {
            itemID = null;
            guid = null;
            durability = 0;
        }

        public ItemPD()
        {
            // Do nothing
        }

        public ItemPD(string guid, ItemConfig itemConfig)
        {
            Utils.Assert(itemConfig != null);

            this.guid = guid;
            this.itemID = itemConfig.id;
            this.durability = itemConfig.durability;
        }

        public virtual ItemPD Clone()
        {
            var obj = new ItemPD();
            obj.guid = guid;
            obj.itemID = itemID;
            obj.durability = durability;
            return obj;
        }

        public virtual void Clone(ItemPD itemPD)
        {
            Utils.Assert(itemPD != null);
            guid = itemPD.guid;
            itemID = itemPD.itemID;
            durability = itemPD.durability;
        }
    }
}
