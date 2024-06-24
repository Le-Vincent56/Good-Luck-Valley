using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.VFX
{
    public class BlurRendererFeature : ScriptableRendererFeature
    {
        BlurRenderPass blurRenderPass;
        public override void Create()
        {
            blurRenderPass = new BlurRenderPass();
            name = "Blur";
        }

        //public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        //{
        //    if (blurRenderPass.Setup(renderer))
        //    {
        //        renderer.EnqueuePass(blurRenderPass);
        //    }
        //}
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            blurRenderPass.Setup(renderer, renderer.cameraColorTargetHandle);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (blurRenderPass.Setup(renderer, renderer.cameraColorTargetHandle))
            {
                renderer.EnqueuePass(blurRenderPass);
            }
        }
    }
}