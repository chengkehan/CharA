
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class RoleConfig
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
        private float _goodness = 0;
        public float goodness
        {
            get
            {
                return _goodness;
            }
        }

        [SerializeField]
        private float _evil = 0;
        public float evil
        {
            get
            {
                return _evil;
            }
        }

        [SerializeField]
        private float _appearance = 0;
        public float appearance
        {
            get
            {
                return _appearance;
            }
        }

        [SerializeField]
        private float _health = 0;
        public float health
        {
            get
            {
                return _health;
            }
        }

        [SerializeField]
        private float _hungry = 0;
        public float hungry
        {
            get
            {
                return _hungry;
            }
        }

        [SerializeField]
        private float _strong = 0;
        public float strong
        {
            get
            {
                return _strong;
            }
        }

        [SerializeField]
        private float _clever = 0;
        public float clever
        {
            get
            {
                return _clever;
            }
        }

        [SerializeField]
        private float _talky = 0;
        public float talky
        {
            get
            {
                return _talky;
            }
        }

        [SerializeField]
        private float _outsight = 0;
        public float outsight
        {
            get
            {
                return _outsight;
            }
        }

        [SerializeField]
        private float _calm = 0;
        public float calm
        {
            get
            {
                return _calm;
            }
        }

        [SerializeField]
        private float _spirit = 0;
        public float spirit
        {
            get
            {
                return _spirit;
            }
        }

        public RoleConfig (string id, string name, float goodness, float evil, float appearance, float health, float hungry, float strong, float clever, float talky, float outsight, float calm, float spirit)
        {
            _id = id;
            _name = name;
            _goodness = goodness;
            _evil = evil;
            _appearance = appearance;
            _health = health;
            _hungry = hungry;
            _strong = strong;
            _clever = clever;
            _talky = talky;
            _outsight = outsight;
            _calm = calm;
            _spirit = spirit;
        }

        public RoleConfig ()
        {
            _id = string.Empty;
            _name = string.Empty;
            _goodness = 0;
            _evil = 0;
            _appearance = 0;
            _health = 0;
            _hungry = 0;
            _strong = 0;
            _clever = 0;
            _talky = 0;
            _outsight = 0;
            _calm = 0;
            _spirit = 0;
        }
    }
}
