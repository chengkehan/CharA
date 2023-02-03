using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameScript
{
    public class CubemapLightingDynamicShadow : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

            public LayerMask layerMask;

            public string[] shaderPassNames = null;

            [RenderingLayersMaskProperty]
            public int renderingLayerMask = 1;
        }

        public Settings settings = new Settings();

        public class CustomRenderPass : ScriptableRenderPass
        {
            private CubemapLightingDynamicShadow rendererFeature = null;

            private string profilerTag = null;

            private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();

            public CustomRenderPass(string profilerTag, CubemapLightingDynamicShadow rendererFeature)
            {
                this.profilerTag = profilerTag;
                this.rendererFeature = rendererFeature;

                if (rendererFeature.settings.shaderPassNames != null)
                {
                    foreach (var passName in rendererFeature.settings.shaderPassNames)
                    {
                        _shaderTagIdList.Add(new ShaderTagId(passName));
                    }
                }
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (renderingData.cameraData.isPreviewCamera || renderingData.cameraData.isSceneViewCamera)
                {
                    return;
                }

                if (rendererFeature == null || rendererFeature.shadowMapRT == null)
                {
                    return;
                }

                if (SceneRenderer.GetInstance() == null)
                {
                    return;
                }

                var activedOne = SceneRenderer.GetInstance().GetActivedOne();
                if (activedOne == null)
                {
                    return;
                }

                if (activedOne.virtualCamera == null)
                {
                    return;
                }

                var cmd = CommandBufferPool.Get(profilerTag);
                var cmd2 = CommandBufferPool.Get(profilerTag);
                {
                    var camData = renderingData.cameraData;
                    var sortingCriteria = camData.defaultOpaqueSortFlags;
                    var drawingSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, sortingCriteria);
                    var filteringSettings = new FilteringSettings(RenderQueueRange.all, rendererFeature.settings.layerMask, (uint)rendererFeature.settings.renderingLayerMask);
                    var renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
                    cmd.SetRenderTarget(rendererFeature.shadowMapRT);
                    cmd.ClearRenderTarget(false, true, Color.white);
                    cmd.SetViewProjectionMatrices(activedOne.virtualCamera.worldToCameraMatrix, activedOne.virtualCamera.projectionMatrix);
                    context.ExecuteCommandBuffer(cmd);
                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);

                    Camera mainCamera = CameraManager.GetInstance().GetMainCamera();
                    cmd2.SetViewProjectionMatrices(mainCamera.worldToCameraMatrix, mainCamera.projectionMatrix);
                    context.ExecuteCommandBuffer(cmd2);
                }
                CommandBufferPool.Release(cmd);
                CommandBufferPool.Release(cmd2);
            }
        }

        private CustomRenderPass m_ScriptablePass;

        private RenderTexture shadowMapRT = null;
        public RenderTexture GetShadowMapRT()
        {
            if (SceneRenderer.GetInstance() == null)
            {
                return null;
            }

            var activedOne = SceneRenderer.GetInstance().GetActivedOne();
            if (activedOne == null)
            {
                return null;
            }

            if (activedOne.virtualCamera == null)
            {
                return null;
            }

            return shadowMapRT;
        }

        public override void Create()
        {
            m_ScriptablePass = new CustomRenderPass("CubemapLightingDynamicShadow", this);
            m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_ScriptablePass);

            if (shadowMapRT == null)
            {
                int rtWidth = 512;
                int rtHeight = 512;
                shadowMapRT = new RenderTexture(rtWidth, rtHeight, 0, RenderTextureFormat.Default);
                shadowMapRT.name = "CubemapLightingShadowmap";
            }
        }

        private void OnDestroy()
        {
            if (shadowMapRT != null)
            {
                DestroyImmediate(shadowMapRT);
                shadowMapRT = null;
            }
        }
    }
}