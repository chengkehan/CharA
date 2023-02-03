using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class ShowFPS : MonoBehaviour
    {
        private float updateInterval = 0.5F;
        private double lastInterval;
        private int frames = 0;
        private float fps;
        void Start()
        {
            lastInterval = Time.realtimeSinceStartup;
            frames = 0;
        }
        void OnGUI()
        {
            GUILayout.Label(fps.ToString("f2") + " " + Screen.width + "x" + Screen.height);
        }
        void Update()
        {
            ++frames;
            float timeNow = Time.realtimeSinceStartup;
            if (timeNow > lastInterval + updateInterval)
            {
                fps = (float)(frames / (timeNow - lastInterval));
                frames = 0;
                lastInterval = timeNow;
            }
        }
    }
}
