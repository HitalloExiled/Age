using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class Framebuffer : Resource<VkFramebuffer>
{
    public required VkExtent2D    Extent     { get; init; }
    public required VkImageView[] ImageViews { get; init; } = [];

    internal Framebuffer(VulkanRenderer renderer, VkFramebuffer value) : base(renderer, value)
    { }

    protected override void OnDispose()
    {
        this.Value.Dispose();

        foreach (var imageView in this.ImageViews)
        {
            imageView.Dispose();
        }
    }
}
