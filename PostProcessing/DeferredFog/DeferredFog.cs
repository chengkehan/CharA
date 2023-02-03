using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameScript
{
    public class DeferredFog : ScriptableRendererFeature
    {
        [System.Serializable]
        public class DeferredFogSettings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

            public Material material = null;
        }

        public DeferredFogSettings settings = new DeferredFogSettings();

        class CustomRenderPass : ScriptableRenderPass
        {
            public DeferredFogSettings settings = null;

            public RenderTargetIdentifier source { get; set; }

            private int sourceId_copy = 0;
            private RenderTargetIdentifier sourceRT_copy;

            private CommandBuffer commandBuffer = null;

            // This method is called before executing the render pass.
            // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
            // When empty this render pass will render to the active camera render target.
            // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
            // The render pipeline will ensure target setup and clearing happens in a performant manner.
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                commandBuffer = CommandBufferPool.Get("DeferredFog");

                var cameraTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                var width = cameraTextureDescriptor.width;
                var height = cameraTextureDescriptor.height;

                sourceId_copy = Shader.PropertyToID("_SourceRT_Copy");
                cmd.GetTemporaryRT(sourceId_copy, width, height, 0, FilterMode.Bilinear, cameraTextureDescriptor.colorFormat);
                sourceRT_copy = new RenderTargetIdentifier(sourceId_copy);
                ConfigureTarget(sourceRT_copy);
            }

            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (settings.material == null || settings.material.shader == null || settings.material.shader.isSupported == false)
                {
                    return;
                }

                commandBuffer.Blit(source, sourceRT_copy);
                commandBuffer.SetGlobalTexture("_DeferredFogMainTex", sourceRT_copy);
                commandBuffer.Blit(sourceRT_copy, source, settings.material, 0);
                context.ExecuteCommandBuffer(commandBuffer);
            }

            // Cleanup any allocated resources that were created during the execution of this render pass.
            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                commandBuffer.Clear();
                CommandBufferPool.Release(commandBuffer);
            }
        }

        CustomRenderPass m_ScriptablePass;

        /// <inheritdoc/>
        public override void Create()
        {
            m_ScriptablePass = new CustomRenderPass();

            // Configures where the render pass should be injected.
            m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
            m_ScriptablePass.settings = settings;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.source = renderer.cameraColorTarget;
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}


