using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class RenderTarget : Disposable
{
    public required Framebuffer Framebuffer { get; init; }

    public VkExtent2D Size => this.Framebuffer.Extent;

    protected override void OnDispose() =>
        this.Framebuffer.Dispose();
}
