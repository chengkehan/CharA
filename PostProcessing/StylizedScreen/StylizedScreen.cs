using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameScript
{
    public class StylizedScreen : ScriptableRendererFeature
    {
        [System.Serializable]
        public class StylizedScreenSettings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

            public Material material = null;
        }

        public StylizedScreenSettings settings = new StylizedScreenSettings();

        public class CustomRenderPass : ScriptableRenderPass
        {
            private string profilerTag = null;

            private RenderTargetIdentifier source { get; set; }

            private StylizedScreen feature = null;

            private static readonly int TempTargetId = Shader.PropertyToID("_TempTargetStylizedScreen");

            public CustomRenderPass(string profilerTag, StylizedScreen feature)
            {
                this.profilerTag = profilerTag;
                this.feature = feature;
            }

            public void Setup(RenderTargetIdentifier source)
            {
                this.source = source;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {

            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (feature.settings.material == null || feature.settings.material.shader == null || feature.settings.material.shader.isSupported == false)
                {
                    return;
                }

                if (renderingData.cameraData.postProcessEnabled == false)
                {
                    return;
                }

                if (URPRendering.GetInstance() == null)
                {
                    Utils.LogWarning("URPRendering Missing. StylizedScreen is disabled.");
                    return;
                }
                MaskGenerator mg = URPRendering.GetInstance().GetRendererFeatures<MaskGenerator>();
                if (!mg.maskRenderTextures.TryGetValue(renderingData.cameraData.camera, out RenderTexture maskRT))
                {
                    return;
                }

                CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
                {
                    int width = renderingData.cameraData.camera.scaledPixelWidth;
                    int height = renderingData.cameraData.camera.scaledPixelHeight;
                    cmd.GetTemporaryRT(TempTargetId, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
                    cmd.Blit(source, TempTargetId);
                    cmd.SetGlobalTexture(MaskGenerator.MaskRTId, maskRT);
                    cmd.Blit(TempTargetId, source, feature.settings.material, 0);
                    cmd.ReleaseTemporaryRT(TempTargetId);
                }
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {

            }
        }

        private CustomRenderPass scriptablePass;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var src = renderer.cameraColorTarget;
            scriptablePass.Setup(src);
            renderer.EnqueuePass(scriptablePass);
        }

        public override void Create()
        {
            scriptablePass = new CustomRenderPass("StylizedScreen", this);
            scriptablePass.renderPassEvent = settings.renderPassEvent;
        }
    }
}
