using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [DisallowMultipleComponent]
    public class SceneRenderer : MonoBehaviour
    {
        private static SceneRenderer s_instance = null;

        public static SceneRenderer GetInstance()
        {
            return s_instance;
        }

        private SceneManager.SceneNames currentSceneName = SceneManager.SceneNames.Undefined;
        private Material[] currentSceneMaterials = null;

        private readonly string[] sceneShaders = new string[] {
            "Custom/ToonShading_Brick", "Custom/ToonShading_Carpet", "Custom/ToonShading_CinderBlock", "Custom/ToonShading_CinderBlockRubble",
            "Custom/ToonShading_CinderBlockRubbleTile", "Custom/ToonShading_Concrete", "Custom/ToonShading_ConcreteRubble",
            "Custom/ToonShading_Door", "Custom/ToonShading_Drywall", "Custom/ToonShading_Glass", "Custom/ToonShading_GlassInterior",
            "Custom/ToonShading_Insulation", "Custom/ToonShading_Plywood", "Custom/ToonShading_Rebar", "Custom/ToonShading_Role",
            "Custom/ToonShading_Rubble", "Custom/ToonShading_RubbleTile", "Custom/ToonShading_Tile", "Custom/ToonShading_Trim",
            "Custom/ToonShading_Wallpaper", "Custom/ToonShading_WoodFloor"
        };

        [SerializeField]
        private Texture2D _gridFadeTex = null;
        public Texture2D gridFadeTex
        {
            get
            {
                return _gridFadeTex;
            }
        }

        private CubemapLightingConfig[] allConfigs = null;

        private int _CubemapLighting_BBoxMin_ID = 0;
        private int _CubemapLighting_BBoxMax_ID = 0;
        private int _CubemapLighting_CenterPos_ID = 0;
        private int _CubemapLighting_LightPos_ID = 0;
        private int _CubemapLightingTex_ID = 0;
        private int _CubemapLighting_DataBundle_ID = 0;
        private int _CubemapLighting_DynamicShadowTex_ID = 0;
        private int _CubemapLighting_DynamicShadowVP_ID = 0;
        private int _CubemapLighting_Role_ID = 0;

        private const string KEYWORDS_CUBEMAP_LIGHTING = "_CUBEMAP_LIGHTING";
        private const string KEYWORDS_DYNAMIC_SHADOW = "_CUBEMAP_LIGHTING_DYNAMIC_SHADOW";

        private CubemapLightingConfig activedOne = null;

        private float fadeInProgress = 0;

        public void RefreshCurrentSceneMaterials()
        {
            if (currentSceneName != SceneManager.GetInstance().sceneName)
            {
                currentSceneName = SceneManager.GetInstance().sceneName;

                var allMaterials = new List<Material>();
                var allRenderers = SceneManager.GetInstance().CurrentSceneNode().gameObject.GetComponentsInChildren<MeshRenderer>(true);
                foreach (var renderer in allRenderers)
                {
                    if (renderer != null)
                    {
                        var materials = renderer.materials;
                        if (materials != null)
                        {
                            foreach (var material in materials)
                            {
                                if (material != null && material.shader != null && Array.IndexOf(sceneShaders, material.shader.name) != -1)
                                {
                                    allMaterials.Add(material);
                                }
                            }
                        }
                    }
                }

                currentSceneMaterials = allMaterials.ToArray();
            }
        }

        public CubemapLightingConfig GetActivedOne()
        {
            return activedOne;
        }

        private void EnableCubemapLightingMaterial(
            Material material, Texture cubemap,
            Vector3 BBoxMin, Vector3 BBoxMax, Vector3 BBoxCenter, Vector3 lightWPos,
            float fadeInProgress, Matrix4x4 shadowVP, bool isActor, float lightingRange, float shadowedRamp,
            Vector3 roleLightingOffset)
        {
            if (material != null)
            {
                material.EnableKeyword(KEYWORDS_CUBEMAP_LIGHTING);
                material.SetTexture(_CubemapLightingTex_ID, cubemap);
                material.SetVector(_CubemapLighting_BBoxMin_ID, BBoxMin);
                material.SetVector(_CubemapLighting_BBoxMax_ID, BBoxMax);
                material.SetVector(_CubemapLighting_CenterPos_ID, BBoxCenter);
                material.SetVector(_CubemapLighting_LightPos_ID, lightWPos);
                material.SetVector(_CubemapLighting_DataBundle_ID, new Vector4(0, fadeInProgress, shadowedRamp, lightingRange));
                material.SetVector(_CubemapLighting_Role_ID, new Vector4(isActor ? 1 : 0, roleLightingOffset.x, roleLightingOffset.y, roleLightingOffset.z));

                if (isActor == false)
                {
                    if (URPRendering.GetInstance() != null)
                    {
                        var rendererFeature = URPRendering.GetInstance().GetRendererFeatures<CubemapLightingDynamicShadow>();
                        if (rendererFeature != null)
                        {
                            material.EnableKeyword(KEYWORDS_DYNAMIC_SHADOW);
                            material.SetMatrix(_CubemapLighting_DynamicShadowVP_ID, shadowVP);
                            material.SetTexture(_CubemapLighting_DynamicShadowTex_ID, rendererFeature.GetShadowMapRT());
                        }
                        else
                        {
                            material.DisableKeyword(KEYWORDS_DYNAMIC_SHADOW);
                        }
                    }
                    else
                    {
                        material.DisableKeyword(KEYWORDS_DYNAMIC_SHADOW);
                    }
                }
                else
                {
                    material.DisableKeyword(KEYWORDS_DYNAMIC_SHADOW);
                }
            }
        }

        private void DisableCubemapLightingMaterial(Material material)
        {
            if (material != null)
            {
                material.DisableKeyword(KEYWORDS_CUBEMAP_LIGHTING);
                material.DisableKeyword(KEYWORDS_DYNAMIC_SHADOW);
                material.SetTexture(_CubemapLightingTex_ID, null);
                material.SetVector(_CubemapLighting_BBoxMin_ID, Vector4.zero);
                material.SetVector(_CubemapLighting_BBoxMax_ID, Vector4.zero);
                material.SetVector(_CubemapLighting_CenterPos_ID, Vector4.zero);
                material.SetVector(_CubemapLighting_LightPos_ID, Vector4.zero);
                material.SetVector(_CubemapLighting_DataBundle_ID, Vector4.zero);
                material.SetTexture(_CubemapLighting_DynamicShadowTex_ID, null);
                material.SetMatrix(_CubemapLighting_DynamicShadowVP_ID, Matrix4x4.identity);
            }
        }

        private void Awake()
        {
            s_instance = this;

            allConfigs = FindObjectsOfType<CubemapLightingConfig>(true);

            _CubemapLighting_BBoxMin_ID = Shader.PropertyToID("_CubemapLighting_BBoxMin");
            _CubemapLighting_BBoxMax_ID = Shader.PropertyToID("_CubemapLighting_BBoxMax");
            _CubemapLighting_CenterPos_ID = Shader.PropertyToID("_CubemapLighting_CenterPos");
            _CubemapLighting_LightPos_ID = Shader.PropertyToID("_CubemapLighting_LightPos");
            _CubemapLightingTex_ID = Shader.PropertyToID("_CubemapLightingTex");
            _CubemapLighting_DataBundle_ID = Shader.PropertyToID("_CubemapLighting_DataBundle");
            _CubemapLighting_DynamicShadowTex_ID = Shader.PropertyToID("_CubemapLighting_DynamicShadowTex");
            _CubemapLighting_DynamicShadowVP_ID = Shader.PropertyToID("_CubemapLighting_DynamicShadowVP");
            _CubemapLighting_Role_ID = Shader.PropertyToID("_CubemapLighting_Role");
        }

        private void Update()
        {
            // Lighting a room that hero is in it.
            if (ActorsManager.GetInstance() != null && ActorsManager.GetInstance().GetHeroActor() != null)
            {
                if (allConfigs != null)
                {
                    CubemapLightingConfig inIt = null;
                    foreach (var aConfig in allConfigs)
                    {
                        if (aConfig != null && aConfig.IsEnabled() && IsActorInTheBonuds(ActorsManager.GetInstance().GetHeroActor(), aConfig))
                        {
                            inIt = aConfig;
                            break;
                        }
                    }
                    if (inIt != activedOne)
                    {
                        fadeInProgress = 1;
                        activedOne = inIt;
                    }
                    UpdateAllMaterials(currentSceneMaterials, inIt, fadeInProgress, false);
                    fadeInProgress -= Time.deltaTime * 0.8f;
                    fadeInProgress = Mathf.Max(fadeInProgress, 0.65f);
                }
            }

            // Lighting all roles whose are in the same room with hero.
            if (ActorsManager.GetInstance() != null)
            {
                for (int actorI = 0; actorI < ActorsManager.GetInstance().NumberActors(); actorI++)
                {
                    var actor = ActorsManager.GetInstance().GetActor(actorI);
                    if (actor != null)
                    {
                        bool isActorInBounds = IsActorInTheBonuds(actor, activedOne);
                        if (isActorInBounds)
                        {
                            UpdateAllMaterials(actor.allMaterials, activedOne, 1, true);
                        }
                        else
                        {
                            UpdateAllMaterials(actor.allMaterials, null, 0, true);
                        }
                    }
                }
            }

            // Check npc is in view field or out of view field.
            // Fade out if outside view field
            // Fade in if inside view field
            // Fade in if npc is at the same room with hero
            if (ActorsManager.GetInstance() != null && GetActivedOne() != null)
            {
                for (int actorI = 0; actorI < ActorsManager.GetInstance().NumberActors(); actorI++)
                {
                    var actor = ActorsManager.GetInstance().GetActor(actorI);
                    if (actor != null && actor != ActorsManager.GetInstance().GetHeroActor())
                    {
                        bool isVisible = false;
                        if (IsActorInTheBonuds(actor, GetActivedOne()))
                        {
                            isVisible = true;
                        }
                        else
                        {
                            for (int viewFieldI = 0; viewFieldI < GetActivedOne().NumberViewFields(); viewFieldI++)
                            {
                                var viewField = GetActivedOne().GetViewField(viewFieldI);
                                if (viewField != null && viewField.IsEnabled() && viewField.CanBeSeenExtendedViewField(actor.GetHeadPosition()))
                                {
                                    isVisible = true;
                                    break;
                                }
                            }
                        }
                        if (isVisible)
                        {
                            actor.GridFadeIn();
                        }
                        else
                        {
                            actor.GridFadeOut();
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            // Reset materials
            UpdateAllMaterials(currentSceneMaterials, null, 0, false);

            s_instance = null;
        }

        private bool IsActorInTheBonuds(Actor actor, CubemapLightingConfig config)
        {
            if (actor == null || config == null || config.cubeBounds == null)
            {
                return false;
            }

            if (CalculateBounds(config, out Vector3 BBoxCenter, out Vector3 BBoxMin, out Vector3 BBoxMax))
            {
                Vector3 actorWPos = actor.GetHeadPosition();
                return actorWPos.x > BBoxMin.x && actorWPos.x < BBoxMax.x &&
                        actorWPos.y > BBoxMin.y && actorWPos.y < BBoxMax.y &&
                        actorWPos.z > BBoxMin.z && actorWPos.z < BBoxMax.z;
            }
            else
            {
                return false;
            }
        }

        // if config is null, cubemap lighting properties will be reset.
        public void UpdateAllMaterials(Material[] materials, CubemapLightingConfig config, float fadeInProgress, bool isActor)
        {
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    UpdateMaterial(material, config, fadeInProgress, isActor);
                }
            }
        }

        private void UpdateMaterial(Material material, CubemapLightingConfig config, float fadeInProgress, bool isActor)
        {
            if (material != null)
            {
                if (CalculateBounds(config, out Vector3 BBoxCenter, out Vector3 BBoxMin, out Vector3 BBoxMax))
                {
                    Matrix4x4 shadowVP =
                        config.virtualCamera == null || isActor ?
                        Matrix4x4.identity :
                        config.virtualCamera.projectionMatrix * config.virtualCamera.worldToCameraMatrix;

                    Vector3 lightWPos = config.pointLight == null ? Vector3.zero : config.pointLight.transform.position;

                    EnableCubemapLightingMaterial(
                        material, config.cubemap, BBoxMin, BBoxMax, BBoxCenter,
                        lightWPos, fadeInProgress, shadowVP, isActor, config.lightingRange, config.shadowedRamp,
                        config.roleLightingOffset);
                }
                else
                {
                    DisableCubemapLightingMaterial(material);
                };
            }
        }

        private bool CalculateBounds(CubemapLightingConfig config, out Vector3 BBoxCenter, out Vector3 BBoxMin, out Vector3 BBoxMax)
        {
            if (config == null)
            {
                BBoxCenter = Vector3.zero;
                BBoxMin = Vector3.zero;
                BBoxMax = Vector3.zero;
                return false;
            }
            else
            {
                BBoxCenter = config.cubeBounds.transform.position;
                Vector3 bboxLenght = config.cubeBounds.transform.localScale;
                BBoxMin = BBoxCenter - bboxLenght / 2;
                BBoxMax = BBoxCenter + bboxLenght / 2;
                return true;
            }
        }
    }
}