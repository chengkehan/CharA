using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameScript
{
    public class GlobalEffect : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public Material material = null;

            public Material blurMaterial = null;

            public Material radiusBlurMaterial = null;

            public GlobalEffectVolume volume = new GlobalEffectVolume();
        }

        public Settings settings = new Settings();

        public class CustomRenderPass : ScriptableRenderPass
        {
            public Settings settings = null;

            public RenderTargetIdentifier source { get; set; }

            private int sourceId_copy = 0;
            private RenderTargetIdentifier sourceRT_copy;

            private int skyboxSnapshotBlurRT = 0;
            private int skyboxSnapshotBlurTempRT1 = 0;
            private int skyboxSnapshotBlurTempRT2 = 0;

            private CommandBuffer commandBuffer = null;

            private int heightFogNoiseBlendStep = 1;
            private float heightFogNoiseBlend = 0;

            private int skyboxSnapshotID = 0;
            private int globalEffectMainTexID = 0;
            private int skyboxFogParamsID = 0;
            private int heightFogNoiseID = 0;
            private int heightFogParamsID = 0;
            private int heightFogParams2ID = 0;
            private int pointLightsPositionsID = 0;
            private int pointLightsColorID = 0;
            private int skyboxOcclusionID = 0;
            private int skyboxSnapshotBlurID = 0;
            private int sktboxSnapshotOffsetID = 0;
            private int skyboxRadiusBlurID = 0;
            private int radiusBlurParamsID = 0;

            private int radiusBlurRT = 0;

            private Material mainMaterial = null;

            // 6 point lights at most.
            // this amount is also fixed in shader.
            // we should change it both in script and shader at the same time.
            private const int NUMBER_POINT_LIGHTS = 6;
            private Vector4[] pointLightsPositions = new Vector4[NUMBER_POINT_LIGHTS]; /* xyz:position, w:range */
            private Vector4[] pointLightsColor = new Vector4[NUMBER_POINT_LIGHTS]; /* xyz:rgb, w:intnsity */
            private List<GlobalEffectPointLight> pointLights = new List<GlobalEffectPointLight>();
            private Actor heroActor = null;
            private int SortingPointLights(GlobalEffectPointLight a, GlobalEffectPointLight b)
            {
                Utils.Assert(heroActor != null);

                var heroPos = heroActor.roleAnimation.GetMotionAnimator().GetPosition();
                var aPos = a.position;
                var bPos = b.position;
                var distToA = Vector3.SqrMagnitude(heroPos - aPos);
                var distToB = Vector3.SqrMagnitude(heroPos - bPos);
                return
                    distToA > distToB ? 1 :
                    distToA < distToB ? -1 : 0;
            }

            public CustomRenderPass(Settings settings)
            {
                this.settings = settings;

                skyboxSnapshotID = Shader.PropertyToID("_SkyboxSnapshot");
                globalEffectMainTexID = Shader.PropertyToID("_GlobalEffectMainTex");
                skyboxFogParamsID = Shader.PropertyToID("_SkyboxFogParams");
                heightFogNoiseID = Shader.PropertyToID("_HeightFogNoise");
                heightFogParamsID = Shader.PropertyToID("_HeightFogParams");
                heightFogParams2ID = Shader.PropertyToID("_HeightFogParams2");
                pointLightsPositionsID = Shader.PropertyToID("_PointLightsPositions");
                pointLightsColorID = Shader.PropertyToID("_PointLightsColor");
                skyboxOcclusionID = Shader.PropertyToID("_SkyboxOcclusion");
                skyboxSnapshotBlurID = Shader.PropertyToID("_SkyboxSnapshotBlur");
                skyboxSnapshotBlurRT = Shader.PropertyToID("_SkyboxSnapshotBlurRT");
                skyboxSnapshotBlurTempRT1 = Shader.PropertyToID("_SkyboxSnapshotTempBlurRT1");
                skyboxSnapshotBlurTempRT2 = Shader.PropertyToID("_SkyboxSnapshotTempBlurRT2");
                sktboxSnapshotOffsetID = Shader.PropertyToID("_offset");
                radiusBlurParamsID = Shader.PropertyToID("_Params");
                radiusBlurRT = Shader.PropertyToID("_RadiusBlurRT");
                skyboxRadiusBlurID = Shader.PropertyToID("_SkyboxRadiusBlur");
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                commandBuffer = CommandBufferPool.Get("GlobalEffect");

                var cameraTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                var width = cameraTextureDescriptor.width;
                var height = cameraTextureDescriptor.height;

                sourceId_copy = Shader.PropertyToID("_SourceRT_Copy");
                cmd.GetTemporaryRT(sourceId_copy, width, height, 0, FilterMode.Bilinear, cameraTextureDescriptor.colorFormat);
                sourceRT_copy = new RenderTargetIdentifier(sourceId_copy);
                ConfigureTarget(sourceRT_copy);
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(sourceId_copy);
                commandBuffer.Clear();
                CommandBufferPool.Release(commandBuffer);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (settings.material == null || settings.material.shader == null || settings.material.shader.isSupported == false)
                {
                    return;
                }

                if (settings.blurMaterial == null || settings.blurMaterial.shader == null || settings.blurMaterial.shader.isSupported == false)
                {
                    return;
                }

                if (settings.radiusBlurMaterial == null || settings.radiusBlurMaterial.shader == null || settings.radiusBlurMaterial.shader.isSupported == false)
                {
                    return;
                }

                if (mainMaterial == null)
                {
                    mainMaterial = new Material(settings.material); 
                }

                if (renderingData.cameraData.postProcessEnabled == false)
                {
                    return;
                }

                if (renderingData.cameraData.isSceneViewCamera || renderingData.cameraData.isPreviewCamera)
                {
                    return;
                }

                if (CameraManager.GetInstance() == null ||
                    CameraManager.GetInstance().skyboxCapturer == null ||
                    CameraManager.GetInstance().skyboxCapturer.GetSnapshot() == null ||
                    CameraManager.GetInstance().skyboxOcclusion == null ||
                    CameraManager.GetInstance().skyboxOcclusion.GetSnapshot() == null)
                {
                    return;
                }

                var volume = settings.volume;
                if (SceneManager.GetInstance() != null)
                {
                    var sceneNode = SceneManager.GetInstance().CurrentSceneNode();
                    if (sceneNode != null && sceneNode.globalEffectVolume != null && sceneNode.globalEffectVolume.overwrite)
                    {
                        volume = sceneNode.globalEffectVolume;
                    }
                }

                if (volume.heightFogNoise == null)
                {
                    return;
                }

                // collect point lights
                {
                    pointLights.Clear();

                    // all point lights in current scene
                    if (SceneManager.GetInstance() != null)
                    {
                        var sceneNode = SceneManager.GetInstance().CurrentSceneNode();
                        if (sceneNode != null)
                        {
                            var sceneBoxBounds = sceneNode.sceneBoxBounds;
                            for (int pointLightI = 0; pointLightI < GlobalEffectPointLight.NumberPointLights(); pointLightI++)
                            {
                                var aPointLight = GlobalEffectPointLight.GetPointLight(pointLightI);
                                if (aPointLight != null && aPointLight.active && sceneBoxBounds.Contains(aPointLight.position))
                                {
                                    pointLights.Add(aPointLight);
                                }
                            }
                        }
                    }

                    // Sorting point lights by distance to hero
                    if (ActorsManager.GetInstance() != null)
                    {
                        heroActor = ActorsManager.GetInstance().GetHeroActor();
                        if (heroActor != null)
                        {
                            pointLights.Sort(SortingPointLights);
                        }
                    }

                    // Fill point lights data
                    for (int pointLightI = 0; pointLightI < NUMBER_POINT_LIGHTS; pointLightI++)
                    {
                        if (pointLightI >= pointLights.Count)
                        {
                            // Fill dummy data
                            pointLightsPositions[pointLightI] = new Vector4(0, 0, 0, 0.01f);
                            pointLightsColor[pointLightI] = new Vector4(0, 0, 0, 0);
                        }
                        else
                        {
                            var aPointLight = pointLights[pointLightI];
                            var aPointLightPos = aPointLight.position;
                            var aPointLightC = aPointLight.color;
                            // Fill point light data
                            pointLightsPositions[pointLightI] = new Vector4(aPointLightPos.x, aPointLightPos.y, aPointLightPos.z, aPointLight.range);
                            pointLightsColor[pointLightI] = new Vector4(aPointLightC.r, aPointLightC.g, aPointLightC.b, aPointLight.intensity);
                        }
                    }
                }

                if (volume.heightFogCoord)
                {
                    mainMaterial.EnableKeyword("_HEIGHT_FOG_COORD");
                }
                else
                {
                    mainMaterial.DisableKeyword("_HEIGHT_FOG_COORD");
                }

                heightFogNoiseBlend += Time.deltaTime * volume.heightFogNoiseBlendSpeed;
                if (heightFogNoiseBlend >= 1)
                {
                    heightFogNoiseBlend = heightFogNoiseBlend - 1.0f;
                    heightFogNoiseBlendStep++;
                    if (heightFogNoiseBlendStep == 5)
                    {
                        heightFogNoiseBlendStep = 1;
                    }
                }
                mainMaterial.DisableKeyword("_HEIGHT_FOG_NOISE_BLEND_12");
                mainMaterial.DisableKeyword("_HEIGHT_FOG_NOISE_BLEND_23");
                mainMaterial.DisableKeyword("_HEIGHT_FOG_NOISE_BLEND_34");
                mainMaterial.DisableKeyword("_HEIGHT_FOG_NOISE_BLEND_41");
                mainMaterial.EnableKeyword(
                    heightFogNoiseBlendStep == 1 ? "_HEIGHT_FOG_NOISE_BLEND_12" :
                    heightFogNoiseBlendStep == 2 ? "_HEIGHT_FOG_NOISE_BLEND_23" :
                    heightFogNoiseBlendStep == 3 ? "_HEIGHT_FOG_NOISE_BLEND_34" :
                    heightFogNoiseBlendStep == 4 ? "_HEIGHT_FOG_NOISE_BLEND_41" : string.Empty/*never accessed*/);

                commandBuffer.Blit(source, sourceRT_copy);

                // GaussianBlur
                {
                    int downsample = volume.blurDownsample;
                    int blurPasses = volume.blurPasses;

                    var cameraTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                    var width = cameraTextureDescriptor.width / downsample;
                    var height = cameraTextureDescriptor.height / downsample;

                    var sourceRT = CameraManager.GetInstance().skyboxCapturer.GetSnapshot();
                    int tmpId1 = skyboxSnapshotBlurTempRT1;
                    int tmpId2 = skyboxSnapshotBlurTempRT2;
                    commandBuffer.GetTemporaryRT(tmpId1, width, height, 0);
                    commandBuffer.GetTemporaryRT(tmpId2, width, height, 0);
                    commandBuffer.GetTemporaryRT(skyboxSnapshotBlurRT, sourceRT.width, sourceRT.height, 0);
                    commandBuffer.SetGlobalFloat(sktboxSnapshotOffsetID, volume.blurOffset);
                    commandBuffer.Blit(sourceRT, tmpId1, settings.blurMaterial);
                    for (var i = 1; i < blurPasses - 1; i++)
                    {
                        commandBuffer.SetGlobalFloat(sktboxSnapshotOffsetID, volume.blurOffset + i);
                        commandBuffer.Blit(tmpId1, tmpId2, settings.blurMaterial);

                        var rttmp = tmpId1;
                        tmpId1 = tmpId2;
                        tmpId2 = rttmp;
                    }
                    commandBuffer.SetGlobalFloat(sktboxSnapshotOffsetID, volume.blurOffset + blurPasses - 1f);
                    commandBuffer.Blit(tmpId1, skyboxSnapshotBlurRT, settings.blurMaterial);
                    commandBuffer.ReleaseTemporaryRT(tmpId1);
                    commandBuffer.ReleaseTemporaryRT(tmpId2);
                }

                // Radius Blur
                {
                    float RadialCenterX = 1f;
                    float RadialCenterY = 1f;
                    var sourceRT = CameraManager.GetInstance().skyboxCapturer.GetSnapshot();
                    commandBuffer.GetTemporaryRT(radiusBlurRT, sourceRT.width, sourceRT.height);
                    commandBuffer.SetGlobalVector(radiusBlurParamsID, new Vector4(volume.radius * 0.02f, volume.radiusIteration, RadialCenterX, RadialCenterY));
                    commandBuffer.Blit(sourceRT, radiusBlurRT, settings.radiusBlurMaterial);
                }

                commandBuffer.SetGlobalVectorArray(pointLightsPositionsID, pointLightsPositions);
                commandBuffer.SetGlobalVectorArray(pointLightsColorID, pointLightsColor);
                commandBuffer.SetGlobalTexture(skyboxRadiusBlurID, radiusBlurRT);
                commandBuffer.SetGlobalTexture(skyboxSnapshotID, CameraManager.GetInstance().skyboxCapturer.GetSnapshot());
                commandBuffer.SetGlobalTexture(skyboxOcclusionID, CameraManager.GetInstance().skyboxOcclusion.GetSnapshot());
                commandBuffer.SetGlobalTexture(skyboxSnapshotBlurID, skyboxSnapshotBlurRT);
                commandBuffer.SetGlobalTexture(globalEffectMainTexID, sourceRT_copy);
                commandBuffer.SetGlobalVector(skyboxFogParamsID, new Vector4(volume.skyboxFogBlendStart, volume.skyboxFogBlendEnd, volume.skyboxFogPow, volume.heightFogMinAtNight));
                commandBuffer.SetGlobalTexture(heightFogNoiseID, volume.heightFogNoise);
                commandBuffer.SetGlobalVector(heightFogParamsID, new Vector4(volume.heightFogStart, volume.heightFogEnd, volume.heightFogTop, volume.heightFogBottom));
                commandBuffer.SetGlobalVector(heightFogParams2ID, new Vector4(volume.heightFogSpeed, volume.heightFogIntensity, volume.heightFogScale, heightFogNoiseBlend));
                commandBuffer.Blit(sourceRT_copy, source, mainMaterial, 0);
                commandBuffer.ReleaseTemporaryRT(skyboxSnapshotBlurRT);
                commandBuffer.ReleaseTemporaryRT(radiusBlurRT);
                context.ExecuteCommandBuffer(commandBuffer);
            }
        }

        private CustomRenderPass m_ScriptablePass = null;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.source = renderer.cameraColorTarget;
            renderer.EnqueuePass(m_ScriptablePass);
        }

        public override void Create()
        {
            m_ScriptablePass = new CustomRenderPass(settings);
            m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }
    }
}
