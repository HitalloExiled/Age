using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public record Framebuffer : Disposable
{
    public required VkFramebuffer Value     { get; init; }
    public required VkImageView   ImageView { get; init; }

    protected override void OnDispose()
    {
        Value.Dispose();
        ImageView.Dispose();
    }
}
