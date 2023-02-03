using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameScript
{
    public class EdgeStroke : ScriptableRendererFeature
    {
        [System.Serializable]
        public class EdgeStrokeSettings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

            [Range(0, 10)]
            public float intensity = 1f;

            public bool debug = false;

            public Material blurMaterial = null;

            public Material sharpenMaterial = null;
        }

        public EdgeStrokeSettings settings = new EdgeStrokeSettings();

        class CustomRenderPass : ScriptableRenderPass
        {
            public EdgeStrokeSettings settings = null;

            string profilerTag;

            private int sourceRTCopy = Shader.PropertyToID("_SourceRT_Copy");

            private int blurRT = Shader.PropertyToID("_BlurRT");

            private int sharpenIntensityID = Shader.PropertyToID("_SharpenIntensity");

            private int sharpenBlurRTID = Shader.PropertyToID("_SharpenBlurRT");

            private RenderTargetIdentifier source { get; set; }

            public void Setup(RenderTargetIdentifier source)
            {
                this.source = source;
            }

            public CustomRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {

            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (settings.blurMaterial == null || settings.sharpenMaterial == null ||
                    settings.blurMaterial.shader == null || settings.sharpenMaterial.shader == null ||
                    settings.blurMaterial.shader.isSupported == false || settings.sharpenMaterial.shader.isSupported == false)
                {
                    return;
                }

                if (renderingData.cameraData.postProcessEnabled == false)
                {
                    return;
                } 

                CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
                {
                    int width = renderingData.cameraData.camera.scaledPixelWidth;
                    int height = renderingData.cameraData.camera.scaledPixelHeight;
                    cmd.GetTemporaryRT(sourceRTCopy, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
                    cmd.GetTemporaryRT(blurRT, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
                    cmd.Blit(source, sourceRTCopy);
                    cmd.Blit(sourceRTCopy, blurRT, settings.blurMaterial, 0);
                    if (settings.debug)
                    {
                        cmd.EnableShaderKeyword("DEBUG");
                    }
                    else
                    {
                        cmd.DisableShaderKeyword("DEBUG");
                    }
                    cmd.SetGlobalFloat(sharpenIntensityID, settings.intensity);
                    cmd.SetGlobalTexture(sharpenBlurRTID, blurRT);
                    cmd.Blit(sourceRTCopy, source, settings.sharpenMaterial, 0);
                    cmd.ReleaseTemporaryRT(sourceRTCopy);
                    cmd.ReleaseTemporaryRT(blurRT);
                }
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
            }
        }

        CustomRenderPass scriptablePass;

        public override void Create()
        {
            scriptablePass = new CustomRenderPass("EdgeStroke");
            scriptablePass.settings = settings;

            scriptablePass.renderPassEvent = settings.renderPassEvent;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var src = renderer.cameraColorTarget;
            scriptablePass.Setup(src);
            renderer.EnqueuePass(scriptablePass);
        }
    }
}


