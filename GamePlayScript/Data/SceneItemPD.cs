using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class SceneItemPD : ItemPD
    {
        [SerializeField]
        private Vector3 _worldPosition = Vector3.zero;
        public Vector3 worldPosition
        {
            set
            {
                _worldPosition = value;
            }
            get
            {
                return _worldPosition;
            }
        }

        public override ItemPD Clone()
        {
            var obj = base.Clone() as SceneItemPD;
            obj.worldPosition = worldPosition;
            return obj;
        }

        public override void Clone(ItemPD itemPD)
        {
            base.Clone(itemPD);

            if (itemPD is SceneItemPD)
            {
                var sceneItemPD = itemPD as SceneItemPD;
                worldPosition = sceneItemPD.worldPosition;
            }
        }
    }
}
