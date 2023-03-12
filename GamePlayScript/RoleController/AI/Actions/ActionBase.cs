using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public abstract class ActionBase : CleverCrow.Fluid.BTs.Tasks.Actions.ActionBase
    {
        private NpcBrain _npcBrain = null;
        protected NpcBrain npcBrain
        {
            get
            {
                if (_npcBrain == null)
                {
                    if (Owner != null)
                    {
                        _npcBrain = Owner.GetComponent<NpcBrain>();
                    }
                }
                return _npcBrain;
            }
        }
    }
}
