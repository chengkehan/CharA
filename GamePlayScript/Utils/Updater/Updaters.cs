using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class Updaters : MonoBehaviour
    {
        private static Updaters s_instance = null;

        private List<IUpdater> updaters = new List<IUpdater>();

        public static Updaters GetInstance()
        {
            return s_instance;
        }

        public void Add(IUpdater updater)
        {
            if (updater != null)
            {
                updaters.Add(updater);
            }
        }

        public bool Contains(IUpdater updater)
        {
            if (updater == null)
            {
                return false;
            }
            else
            {
                return updaters.Contains(updater);
            }
        }

        public void Add(IUpdater.UpdateHandle updateHandle)
        {
            if (updateHandle == null)
            {
                var updater = new Updater(updateHandle);
                updaters.Add(updater);
            }
        }

        private void Awake()
        {
            s_instance = this;
        }

        private void OnDestroy()
        {
            s_instance = null;
        }

        private void Update()
        {
            var numHandles = updaters.Count;
            for (int i = 0; i < numHandles; i++)
            {
                var updater = updaters[i];
                var result = updater.Update();
                if (result == false)
                {
                    updaters.RemoveAt(i);
                    --numHandles;
                    --i;
                }
            }
        }
    }
}
