using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameScript.Cutscene;

namespace GameScript
{
    public class BreakWallSkill
    {
        // Who is breaking wall
        private Actor _actor = null;
        public Actor actor
        {
            private set
            {
                _actor = value;
            }
            get
            {
                return _actor;
            }
        }

        // target object
        private BreakWall _breakWall = null;
        public BreakWall breakWall
        {
            private set
            {
                _breakWall = value;
            }
            get
            {
                return _breakWall;
            }
        }

        private Action<BreakWallSkill> completeCB = null;

        private Action<BreakWallSkill> itemBrokenCB = null;

        public BreakWallSkill(Actor actor, BreakWall breakWall, Action<BreakWallSkill> completeCB, Action<BreakWallSkill> itemBrokenCB)
        {
            this.actor = actor;
            this.breakWall = breakWall;
            this.completeCB = completeCB;
            this.itemBrokenCB = itemBrokenCB;
        }

        public void Update()
        {
            if (actor.HasInHandItem())
            {
                var inHandItem = actor.pd.inHandItem;
                var itemConfig = DataCenter.GetInstance().GetItemConfig(inHandItem.itemID);
                breakWall.pd.health = Mathf.Max(0, breakWall.pd.health - itemConfig.damage * Time.deltaTime);
                actor.pd.inHandItem.durability = Mathf.Max(0, actor.pd.inHandItem.durability - breakWall.durabilityCost * Time.deltaTime);

                if (breakWall.pd.health <= 0)
                {
                    completeCB(this);
                }
                if (actor.pd.inHandItem.durability <= 0)
                {
                    itemBrokenCB(this);
                }
            }
        }
    }
}
