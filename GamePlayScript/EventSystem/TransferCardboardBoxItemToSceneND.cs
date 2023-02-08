using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class TransferCardboardBoxItemToSceneND : NotificationData
    {
        public string cardboardBoxGUID = null;

        public string itemGUID = null;

        public Vector3 dropPosition = Vector3.zero;
    }
}
