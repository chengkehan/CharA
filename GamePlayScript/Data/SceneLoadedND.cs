using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class SceneLoadedND : NotificationData
    {
        public SceneManager.SceneNames loadedSceneName = SceneManager.SceneNames.Undefined;
    }
}
