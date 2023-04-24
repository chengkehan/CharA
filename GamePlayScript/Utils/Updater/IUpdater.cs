using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public interface IUpdater
    {
        // handle will be removed while it return false, otherwise it works all the time.
        public delegate bool UpdateHandle();

        bool Update();
    }
}
