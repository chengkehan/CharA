using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class PickUpSceneItemND : NotificationData
    {
        // Who picks up this item
        public string roleID = null;

        // guid of item's instance
        public string itemGUID = null;
    }
}
