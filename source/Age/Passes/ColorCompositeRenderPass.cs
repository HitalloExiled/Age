using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age.Passes;

public class ColorCompositeRenderPass(ReadOnlySpan<RenderPass> passes) : CompositeRenderPass(passes)
{
    public override string Name => nameof(ColorCompositeRenderPass);

    public override RenderTarget  RenderTarget  => this.Viewport!.RenderTarget;
    public override CommandBuffer CommandBuffer => VulkanRenderer.Singleton.CurrentCommandBuffer;
}
