using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryboardCore
{
    public enum StoryNodeState
    {
        Idle, // Indicate StoryThread never reached this node.
        Played, // Indicate node was already played.
        Playing // Node is playing, and waiting for complete.
    }
}
