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
            One = 1,
            Two = 2
        }

        public enum Layers
        {
            Interactive3D = 6,
            UI3D = 7
        }

        public enum LayersMask
        {
            Interactive3D = 1 << Layers.Interactive3D,
            UI3D = 1 << Layers.UI3D
        }

        public readonly string HeroRoleID = "200000";

        public readonly float SceneItemYOffset = 1;

        public readonly float SceneItemVisibleRange = 3;

        public readonly float SceneItemPickupRange = 1.5f;
    }
}
