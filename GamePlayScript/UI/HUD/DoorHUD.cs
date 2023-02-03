using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;
using GameScript.Cutscene;

namespace GameScript.UI.HUD
{
    public class DoorHUD : MonoBehaviour
    {
        [SerializeField]
        private GameObject _normalIcon = null;
        private GameObject normalIcon
        {
            get
            {
                return _normalIcon;
            }
        }

        private Door _door = null;
        public Door door
        {
            set
            {
                _door = value;
            }
            get
            {
                return _door;
            }
        }
    }
}
