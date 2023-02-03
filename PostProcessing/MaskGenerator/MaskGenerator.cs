using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameScript
{
    public class MaskGenerator : ScriptableRendererFeature
    {
        public static readonly int MaskRTId = Shader.PropertyToID("_LayeredMaskRT");

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

        // Never change it outside this script
        [System.NonSerialized]
        public Dictionary<Camera, RenderTexture> maskRenderTextures = new Dictionary<Camera, RenderTexture>();

        public class CustomRenderPass : ScriptableRenderPass
        {
            private string profilerTag = null;

            private MaskGenerator maskGenerator = null;

            private static readonly int MaskTargetId = Shader.PropertyToID("_MaskTargetMaskGenerator");

            private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();

            public CustomRenderPass(string profilerTag, MaskGenerator maskGenerator)
            {
                this.profilerTag = profilerTag;
                this.maskGenerator = maskGenerator;

                if (maskGenerator.settings.shaderPassNames != null)
                {
                    foreach (var passName in maskGenerator.settings.shaderPassNames)
                    {
                        _shaderTagIdList.Add(new ShaderTagId(passName));
                    }
                }
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (maskGenerator == null || maskGenerator.maskRenderTextures == null || maskGenerator.targetCamera == null)
                {
                    return;
                }

                if (maskGenerator.maskRenderTextures.TryGetValue(maskGenerator.targetCamera, out RenderTexture rt))
                {
                    ConfigureTarget(rt);
                    ConfigureClear(ClearFlag.All, Color.black);
                }
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get(profilerTag);
                var camData = renderingData.cameraData;
                var sortingCriteria = camData.defaultOpaqueSortFlags;
                var drawingSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, sortingCriteria);
                var filteringSettings = new FilteringSettings(RenderQueueRange.all, maskGenerator.settings.layerMask, (uint)maskGenerator.settings.renderingLayerMask);
                var renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
                context.ExecuteCommandBuffer(cmd);
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {

            }
        }

        private CustomRenderPass scriptablePass;

        private Camera targetCamera = null;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(scriptablePass);

            targetCamera = renderingData.cameraData.camera;
            int width = targetCamera.scaledPixelWidth;
            int height = targetCamera.scaledPixelHeight;

            bool newRT = true;
            if (maskRenderTextures.TryGetValue(targetCamera, out RenderTexture rt))
            {
                if (rt != null)
                {
                    if (rt.width != width || rt.height != height)
                    {
                        DestroyImmediate(rt);
                        maskRenderTextures.Remove(targetCamera);
                    }
                    else
                    {
                        newRT = false;
                    }
                }
                else
                {
                    maskRenderTextures.Remove(targetCamera);
                }
            }

            if (newRT)
            {
                rt = new RenderTexture(width, height, 24, RenderTextureFormat.Default);
                rt.name = "MaskGenerator " + targetCamera.name;
                maskRenderTextures.Add(targetCamera, rt);
            }
        }

        public override void Create()
        {
            scriptablePass = new CustomRenderPass("MaskGenerator", this);
            scriptablePass.renderPassEvent = settings.renderPassEvent;
        }

        private void OnDestroy()
        {
            if (targetCamera != null)
            {
                if (maskRenderTextures.TryGetValue(targetCamera, out RenderTexture rt))
                {
                    DestroyImmediate(rt);
                    maskRenderTextures.Remove(targetCamera);
                }
                targetCamera = null;
            }
        }
    }
}
