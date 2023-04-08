using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.Cutscene;

namespace GameScript
{
    [DisallowMultipleComponent]
    public class Actor : SerializableMonoBehaviour<ActorPD>
    {
        [SerializeField]
        private bool _isHero = false;
        public bool isHero
        {
            get
            {
                return _isHero;
            }
        }

        [SerializeField]
        private Transform _mainHeadTransform = null;
        private Transform mainHeadTransform
        {
            get
            {
                return _mainHeadTransform;
            }
        }

        [SerializeField]
        private Transform _leftHandTransform = null;
        private Transform leftHandTransform
        {
            get
            {
                return _leftHandTransform;
            }
        }

        [SerializeField]
        private Transform _rightHandTransform = null;
        private Transform rightHandTransform
        {
            get
            {
                return _rightHandTransform;
            }
        }

        [SerializeField]
        private Transform _headPointTransform = null;
        private Transform headPointTransform
        {
            get
            {
                return _headPointTransform;
            }
        }

        // Never change this outside CubemapLighting module
        [System.NonSerialized]
        public Material[] allMaterials = null;

        private RoleAnimation _roleAnimation = null;
        public RoleAnimation roleAnimation
        {
            private set
            {
                _roleAnimation = value;
            }
            get
            {
                return _roleAnimation;
            }
        }

        private Renderer[] allRenderers = null;

        private Collider[] allColliders = null;

        private float gridFadeValue = 1;
        private const string GRID_FADE_KEY_WORD = "_GRID_FADE";
        private int _GridFadeAlpha_ID = 0;
        private int _GridFadeTex_ID = 0;
        private int gridFadeDirection = 0; // 1:fadeIn, -1:fadeOut, 0:doNothing
        private float gridFadeSpeed = 5;

        public void TalkingBubble(string txtKey, float duration = 1)
        {
            TalkingBubbleText(DataCenter.GetInstance().GetLanguage(txtKey), duration);
        }

        public void TalkingBubbleText(string txt, float duration = 1)
        {
            var notificationData = new TalkingBubbleND();
            notificationData.actorGUID = guid;
            notificationData.talkingText = txt;
            notificationData.duration = duration;
            EventSystem.GetInstance().Notify(EventID.TalkingBubble, notificationData);
        }

        public bool HasInHandItem()
        {
            return pd.inHandItem.IsEmpty() == false;
        }

        public void DeleteInHandItem()
        {
            AssetsManager.GetInstance().UnloadSceneItem(pd.inHandItem.guid);
            pd.inHandItem.SetEmpty();
            roleAnimation.GetMotionAnimator().SetUpBodyAnimation(MotionAnimator.UpBodyAnimation.None);
        }

        public void SetInHandItem(ItemPD itemPD)
        {
            pd.inHandItem.Clone(itemPD);
            roleAnimation.GetMotionAnimator().SetUpBodyAnimation(MotionAnimator.UpBodyAnimation.StickInHands);

            var inHandItemGo = AssetsManager.GetInstance().LoadSceneItem(itemPD.guid, itemPD.itemID);
            var item = inHandItemGo.GetComponent<Item>();
            item.SetPhysics(false);
            item.SetParent(transform);
            item.Binding(this);
        }

        public void SetPocketItem(Define.PocketType pocketType, ItemPD itemPD)
        {
            pd.GetPocketItem((int)pocketType).Clone(itemPD);
        }

        public void DeletePocketItem(string itemGUID)
        {
            for (int i = 0; i < pd.NumberPocketItems(); i++)
            {
                if (pd.GetPocketItem(i).guid == itemGUID)
                {
                    pd.GetPocketItem(i).SetEmpty();
                }
            }
        }

        public override void Save()
        {
            base.Save();

            SavePosition();
        }

        private void SavePosition()
        {
            pd.position = roleAnimation.GetMotionAnimator().GetPosition();
        }

        public bool IsEnabled()
        {
            return this.enabled;
        }

        public bool IsVisible()
        {
            return gridFadeValue > 0.99f;
        }

        public string GetId()
        {
            return gameObject == null ? string.Empty : gameObject.name;
        }

        public void GridFadeIn()
        {
            gridFadeDirection = 1;
        }

        public void GridFadeOut()
        {
            gridFadeDirection = -1;
        }

        public Vector3 GetHeadPointPosition()
        {
            Utils.LogObservably("headPointTransform is null", headPointTransform == null);
            return headPointTransform == null ? Vector3.zero : headPointTransform.position;
        }

        public Vector3 GetHeadDirection()
        {
            Utils.LogObservably("mainHeadTransform is null", mainHeadTransform == null);
            return mainHeadTransform == null ? Vector3.one : mainHeadTransform.forward;
        }

        public Vector3 GetHeadPosition()
        {
            Utils.LogObservably("mainHeadTransform is null", mainHeadTransform == null);
            return mainHeadTransform == null ? Vector3.zero : mainHeadTransform.position;
        }

        public Vector3 GetLeftHandPosition()
        {
            Utils.LogObservably("leftHandTransform is null", leftHandTransform == null);
            return leftHandTransform == null ? Vector3.zero : leftHandTransform.position;
        }

        public Vector3 GetRightHandPosition()
        {
            Utils.LogObservably("rightHeadTransform is null", rightHandTransform == null);
            return rightHandTransform == null ? Vector3.zero : rightHandTransform.position;
        }

        public void AttachToRoot(Transform root)
        {
            if (root != null && transform != null)
            {
                transform.parent = root;
            }
        }

        protected override void InitializeOnAwake()
        {
            base.InitializeOnAwake();

            roleAnimation = gameObject.GetComponent<RoleAnimation>();
            Utils.Assert(roleAnimation != null);
            roleAnimation.actor.o = this;

            var allMaterials = new List<Material>();
            allRenderers = GetComponentsInChildren<Renderer>(true);
            if (allRenderers != null)
            {
                foreach (var renderer in allRenderers)
                {
                    if (renderer != null && renderer.sharedMaterial != null)
                    {
                        allMaterials.Add(renderer.material);
                    }
                }
            }
            this.allMaterials = allMaterials.ToArray();

            allColliders = GetComponentsInChildren<Collider>();

            _GridFadeAlpha_ID = Shader.PropertyToID("_GridFadeAlpha");
            _GridFadeTex_ID = Shader.PropertyToID("_GridFadeTex");
        }

        private void Update()
        {
            if (gridFadeDirection != 0)
            {
                if (gridFadeDirection == 1)
                {
                    if (gridFadeValue < 1)
                    {
                        gridFadeValue = Mathf.Min(gridFadeValue + gridFadeDirection * Time.deltaTime * gridFadeSpeed, 1);
                        UpdateMaterialGridFade(true);
                        SetRenderersEnabled(true);
                        SetCollidersEnabled(true);
                    }
                    else
                    {
                        gridFadeDirection = 0;
                        gridFadeValue = 1;
                        UpdateMaterialGridFade(false);
                        SetRenderersEnabled(true);
                        SetCollidersEnabled(true);
                    }
                }
                else if (gridFadeDirection == -1)
                {
                    if (gridFadeValue > 0)
                    {
                        gridFadeValue = Mathf.Max(gridFadeValue + gridFadeDirection * Time.deltaTime * gridFadeSpeed, 0);
                        UpdateMaterialGridFade(true);
                        SetRenderersEnabled(true);
                        SetCollidersEnabled(true);
                    }
                    else
                    {
                        gridFadeDirection = 0;
                        gridFadeValue = 0;
                        UpdateMaterialGridFade(false);
                        SetRenderersEnabled(false);
                        SetCollidersEnabled(false);
                    }
                }
                else
                {
                    Utils.Assert(false);
                }
            }

            SavePosition();
        }

        private void UpdateMaterialGridFade(bool isEnabled)
        {
            if (allMaterials != null)
            {
                foreach (var mtrl in allMaterials)
                {
                    if (mtrl != null)
                    {
                        if (isEnabled)
                        {
                            mtrl.EnableKeyword(GRID_FADE_KEY_WORD);
                            mtrl.SetFloat(_GridFadeAlpha_ID, gridFadeValue);
                            if (SceneRenderer.GetInstance() != null)
                            {
                                mtrl.SetTexture(_GridFadeTex_ID, SceneRenderer.GetInstance().gridFadeTex);
                            }
                        }
                        else
                        {
                            mtrl.DisableKeyword(GRID_FADE_KEY_WORD);
                        }
                    }
                }
            }
        }

        private void SetRenderersEnabled(bool isEnabled)
        {
            if (allRenderers != null)
            {
                foreach (var renderer in allRenderers)
                {
                    if (renderer != null)
                    {
                        if (renderer.enabled != isEnabled)
                        {
                            renderer.enabled = isEnabled;
                        }
                    }
                }
            }
        }

        private void SetCollidersEnabled(bool isEnabled)
        {
            if (allColliders != null)
            {
                foreach (var collider in allColliders)
                {
                    if (collider != null)
                    {
                        if (collider.enabled != isEnabled)
                        {
                            collider.enabled = isEnabled;
                        }
                    }
                }
            }
        }
    }
}