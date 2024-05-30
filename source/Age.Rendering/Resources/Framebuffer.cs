using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class Framebuffer : Disposable
{
    public required VkFramebuffer Value      { get; init; }
    public required VkImageView[] ImageViews { get; init; } = [];

    protected override void OnDispose()
    {
        this.Value.Dispose();

        foreach (var imageView in this.ImageViews)
        {
            imageView.Dispose();
        }
    }
}
