using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class Updater : IUpdater
    {
        private IUpdater.UpdateHandle updateHandle = null;

        public Updater(IUpdater.UpdateHandle updateHandle)
        {
            this.updateHandle = updateHandle;
        }

        public bool Update()
        {
            if (updateHandle == null)
            {
                return false;
            }
            else
            {
                return updateHandle();
            }
        }
    }
}
