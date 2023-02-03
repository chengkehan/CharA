using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class Define
    {
        public enum PocketType
        {
            Clothes_Left_Side = 0,
            Clothes_Right_Side = 1,
            Trousers_Left_Side = 2,
            Trousers_Right_Side = 3
        }

        // How many space does item take up in pocket or backpack. 
        public enum ItemSpace
        {
            Zero = 0,
            One = 1,
            Four = 4
        }

        public readonly float SceneItemYOffset = 1;

        public readonly float SceneItemVisibleRange = 3;

        public readonly float SceneItemPickupRange = 1.5f;
    }
}
