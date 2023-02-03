using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class SequencePlayer : SerializableMonoBehaviour<SequencePlayerPD>
    {
        public enum ActiveType
        {
            Manually,
            OnEnterBounds,
            OnExitBounds
        }

        public enum OperationType
        {
            Undefined, 
            Active,
            Deactive
        }

        [Serializable]
        public class Animation
        {
            public OperationType startOperationType = OperationType.Active;

            public OperationType endOperationType = OperationType.Active;

            public GameObject target = null;

            public float startTime = 0;

            public float dureciton = 0;

            public void PlayToEndWithoutProgress()
            {
                if (target != null)
                {
                    var anims = target.GetComponentsInChildren<UnityEngine.Animation>();
                    if (anims != null)
                    {
                        foreach (var anim in anims)
                        {
                            if (anim != null)
                            {
                                foreach (AnimationState animState in anim)
                                {
                                    animState.normalizedTime = 1;
                                }
                                anim.Sample();
                                anim.Stop();
                            }
                        }
                    }
                }
            }
        }

        [SerializeField]
        private ActiveType _activeType = ActiveType.Manually;
        public ActiveType activeType
        {
            get
            {
                return _activeType;
            }
        }

        [SerializeField]
        private Vector3 _boundsPosition = Vector3.zero;
        private Vector3 boundsPosition
        {
            get
            {
                return _boundsPosition;
            }
        }

        [SerializeField]
        private Vector3 _boundsSize = Vector3.one;
        private Vector3 boundsSize
        {
            get
            {
                return _boundsSize;
            }
        }

        [SerializeField]
        private Animation[] _animations = null;
        public Animation[] animations
        {
            get
            {
                return _animations;
            }
        }

        private float time = 0;

        private Bounds bounds = new Bounds();

        private bool isHeroInBounds = false;

        private bool playing = false;

        protected override void InitializeOnStart()
        {
            base.InitializeOnStart();

            if (pd.isPlayed)
            {
                PlayToEndWithoutProgress();
            }
        }

        private void PlayToEndWithoutProgress()
        {
            if (animations != null)
            {
                foreach (var animation in animations)
                {
                    RefreshActive(animation);
                    animation.PlayToEndWithoutProgress();
                }
            }
        }

        public bool Play()
        {
            if (pd.isPlayed)
            {
                return false;
            }

            pd.isPlayed = true;
            time = 0;
            playing = true;

            return true;
        }

        private void Update()
        {
            bounds.center = transform.position + boundsPosition;
            bounds.size = boundsSize;

            var isHeroInBounds = IsHeroInBounds();
            if (isHeroInBounds)
            {
                if (this.isHeroInBounds == false)
                {
                    this.isHeroInBounds = true;
                    OnEnterBounds();
                }
            }
            else
            {
                if (this.isHeroInBounds)
                {
                    this.isHeroInBounds = false;
                    OnExitBounds();
                }
            }
        }

        private void OnEnterBounds()
        {
            if (activeType == ActiveType.OnEnterBounds)
            {
                Play();
            }
        }

        private void OnExitBounds()
        {
            if (activeType == ActiveType.OnExitBounds)
            {
                Play();
            }
        }

        private bool IsHeroInBounds()
        {
            if (ActorsManager.GetInstance() != null && ActorsManager.GetInstance().GetHeroActor() != null)
            {
                return bounds.Contains(ActorsManager.GetInstance().GetHeroActor().roleAnimation.GetMotionAnimator().GetPosition());
            }
            else
            {
                return false;
            }
        }

        private void FixedUpdate()
        {
            if (playing)
            {
                time += Time.fixedDeltaTime;

                int completedAnimationsCount = 0;
                int numAnimations = animations == null ? 0 : animations.Length;

                if (numAnimations > 0)
                {
                    for (int i = 0; i < numAnimations; i++)
                    {
                        var animation = animations[i];
                        if (time >= animation.startTime && time <= animation.startTime + animation.dureciton)
                        {
                            RefreshActive(animation);
                        }
                        if (time > animation.startTime + animation.dureciton)
                        {
                            ++completedAnimationsCount;
                            RefreshActive(animation);
                        }
                    }
                }

                if (completedAnimationsCount == animations.Length)
                {
                    playing = false;
                }
            }
        }

        private void RefreshActive(Animation animation)
        {
            if (animation.startOperationType == OperationType.Active || animation.endOperationType == OperationType.Active)
            {
                SetActive(animation.target, true);
            }
            if (animation.startOperationType == OperationType.Deactive || animation.endOperationType == OperationType.Deactive)
            {
                SetActive(animation.target, false);
            }
        }

        private void SetActive(GameObject target, bool isActive)
        {
            if (target != null && target.activeSelf != isActive)
            {
                target.SetActive(isActive);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (activeType == ActiveType.OnEnterBounds || activeType == ActiveType.OnExitBounds)
            {
                Gizmos.color = Color.white;
                Gizmos.matrix = Matrix4x4.identity;

                Color gizmosColor = Gizmos.color;
                {
                    Gizmos.color = IsHeroInBounds() ? Color.green : Color.white;
                    Gizmos.DrawWireCube(transform.position + boundsPosition, boundsSize);
                }
                Gizmos.color = gizmosColor;
            }
        }
#endif
    }
}
