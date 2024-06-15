using Age.Rendering.Vulkan;

namespace Age.Rendering.Resources;

public class RenderPipeline(VulkanRenderer renderer) : Resource(renderer)
{
    public required RenderPass  RenderPass  { get; init; }
    public required Framebuffer Framebuffer { get; init; }

    protected override void OnDispose()
    {
        this.RenderPass.Dispose();
        this.Framebuffer.Dispose();
    }
}
