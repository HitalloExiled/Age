using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class RenderTarget(Texture texture, Framebuffer framebuffer) : Resource
{
    public Texture     Texture     { get; } = texture;
    public Framebuffer Framebuffer { get; } = framebuffer;

    public VkExtent2D Size => this.Framebuffer.Extent;

    protected override void OnDispose()
    {
        this.Framebuffer.Dispose();
        this.Texture.Dispose();
    }
}
