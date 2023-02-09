using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class NotificationData
    {
        private bool _interrupted = false;
        public bool interrupted
        {
            get
            {
                return _interrupted;
            }
            set
            {
                _interrupted = value;
            }
        }
    }
}
