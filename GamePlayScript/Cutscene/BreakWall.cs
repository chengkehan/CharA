using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.Cutscene;

namespace GameScript.Cutscene
{
    public class BreakWall : SerializableMonoBehaviour<BreakWallPD>
    {
        [SerializeField]
        private SequencePlayer _sequencePlayer = null;
        public SequencePlayer sequencePlayer
        {
            get
            {
                return _sequencePlayer;
            }
        }

        [SerializeField]
        private DynamicLink _dynamicLink = null;
        public DynamicLink dynamicLink
        {
            get
            {
                return _dynamicLink;
            }
        }

        [SerializeField]
        private PairsData _itemNeeded = null;
        public PairsData itemNeeded
        {
            get
            {
                return _itemNeeded;
            }
        }

        [Tooltip("When health is zero, it's broken.")]
        [SerializeField]
        [Min(0)]
        private float _totalHealth = 0;
        public float totalHealth
        {
            get
            {
                return _totalHealth;
            }
        }

        [Tooltip("Costing durability of item per second.")]
        [SerializeField]
        [Min(0)]
        private float _durabilityCost = 0;
        public float durabilityCost
        {
            get
            {
                return _durabilityCost;
            }
        }

        #region hud bonuds

        [Tooltip("HUD visible when hero in this bounds.")]
        [SerializeField]
        private Vector3 _hudBoundsPosition = Vector3.zero;

        [Tooltip("HUD visible when hero in this bounds.")]
        [SerializeField]
        private Vector3 _hudBoundsSize = Vector3.one;

        private Bounds _hudBounds = new Bounds();
        private Bounds hudBounds
        {
            get
            {
                _hudBounds.center = GetPosition() + _hudBoundsPosition;
                _hudBounds.size = _hudBoundsSize;
                return _hudBounds;
            }
        }

        public bool InHUDBounds(Vector3 pos)
        {
            return hudBounds.Contains(pos);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Color gizmosColor = Gizmos.color;
            {
                Gizmos.color = Color.gray;
                var bounds = hudBounds;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
            Gizmos.color = gizmosColor;
        }
#endif

        #endregion

        public Vector3 GetPosition()
        {
            return transform == null ? Vector3.zero : transform.position;
        }

        public void Collapse()
        {
            sequencePlayer.Play();
            dynamicLink.Link();
        }

        protected override void InitializeOnAwake()
        {
            base.InitializeOnAwake();

            if (pd.initialized.o == false)
            {
                pd.initialized.o = true;
                pd.health = totalHealth;
            }
        }

        protected override void InitializeOnStart()
        {
            base.InitializeOnStart();

            //if (DataCenter.query.IsWallBreaked(pd))
            //{
            //    sequencePlayer.PlayToEndWithoutProgress();
            //    dynamicLink.Link();
            //}
        }
    }
}
