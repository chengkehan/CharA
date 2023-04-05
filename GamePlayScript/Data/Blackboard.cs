using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    // Data and state shared with all scripts.
    public class Blackboard
    {
        // Hero do some actions solo and mute other ones.(No Player Controlling, No HUD Showing, No Trigger Detection, etc.)
        public bool heroSoloAndMuteOthers = false;
    }
}
