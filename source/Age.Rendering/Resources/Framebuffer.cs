using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class Framebuffer : Resource<VkFramebuffer>
{
    public required VkExtent2D    Extent     { get; init; }
    public required VkImageView[] ImageViews { get; init; } = [];

    public override VkFramebuffer Instance { get; }

    internal Framebuffer(VkFramebuffer instance) =>
        this.Instance = instance;

    protected override void Disposed()
    {
        this.Instance.Dispose();

        foreach (var imageView in this.ImageViews)
        {
            imageView.Dispose();
        }
    }
}
