using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class DestroyItemND : NotificationData
    {
        // item owner
        public string actorGUID = null;

        public string itemGUID = null;
    }
}
