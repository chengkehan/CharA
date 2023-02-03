using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class DropItemToSceneND : NotificationData
    {
        // Who drop this item
        public string actorGUID = null;

        // guid of item's instance
        public string itemGUID = null;
    }
}
