using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public partial record RenderPass : Disposable
{
    public required VkRenderPass  Value        { get; init; }
    public required Framebuffer[] Framebuffers { get; init; }
    public required VkExtent2D    Extent       { get; init; }

    protected override void OnDispose()
    {
        this.Value.Dispose();

        foreach (var frameBuffer in this.Framebuffers)
        {
            frameBuffer.Dispose();
        }
    }
}
