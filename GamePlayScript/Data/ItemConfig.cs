
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class ItemConfig
    {

        [SerializeField]
        private string _id = string.Empty;
        public string id
        {
            get
            {
                return _id;
            }
        }

        [SerializeField]
        private string _name = string.Empty;
        public string name
        {
            get
            {
                return _name;
            }
        }

        [SerializeField]
        private float _damage = 0;
        public float damage
        {
            get
            {
                return _damage;
            }
        }

        [SerializeField]
        private float _durability = 0;
        public float durability
        {
            get
            {
                return _durability;
            }
        }

        [SerializeField]
        private string _anim = string.Empty;
        public string anim
        {
            get
            {
                return _anim;
            }
        }

        [SerializeField]
        private bool _eatable = false;
        public bool eatable
        {
            get
            {
                return _eatable;
            }
        }

        [SerializeField]
        private int _strength_add = 0;
        public int strength_add
        {
            get
            {
                return _strength_add;
            }
        }

        [SerializeField]
        private int _space = 0;
        public int space
        {
            get
            {
                return _space;
            }
        }

        [SerializeField]
        private string _description = string.Empty;
        public string description
        {
            get
            {
                return _description;
            }
        }

        public ItemConfig (string id, string name, float damage, float durability, string anim, bool eatable, int strength_add, int space, string description)
        {
            _id = id;
            _name = name;
            _damage = damage;
            _durability = durability;
            _anim = anim;
            _eatable = eatable;
            _strength_add = strength_add;
            _space = space;
            _description = description;
        }

        public ItemConfig ()
        {
            _id = string.Empty;
            _name = string.Empty;
            _damage = 0;
            _durability = 0;
            _anim = string.Empty;
            _eatable = false;
            _strength_add = 0;
            _space = 0;
            _description = string.Empty;
        }
    }
}
