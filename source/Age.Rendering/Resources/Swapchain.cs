using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public record Swapchain : Disposable
{
    public required VkExtent2D      Extent       { get; init; }
    public required VkFormat        Format       { get; init; }
    public required VkFramebuffer[] Framebuffers { get; init; }
    public required VkImage[]       Images       { get; init; }
    public required VkImageView[]   ImageViews   { get; init; }
    public required VkRenderPass    RenderPass   { get; init; }
    public required VkSwapchainKHR  Value        { get; init; }

    protected override void OnDispose()
    {
        for (var i = 0; i < this.ImageViews.Length; i++)
        {
            this.Framebuffers[i].Dispose();
            this.ImageViews[i].Dispose();
        }

        this.RenderPass.Dispose();
        this.Value.Dispose();
    }
}
