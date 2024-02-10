using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Vulkan.Handlers;

public record SwapchainHandler
{
    public required VkExtent2D      Extent        { get; init; }
    public required VkFormat        Format        { get; init; }
    public required VkFramebuffer[] Framebuffers  { get; init; }
    public required VkSwapchainKHR  Handler       { get; init; }
    public required VkImage[]       Images        { get; init; }
    public required VkImageView[]   ImageViews    { get; init; }
    public required VkRenderPass    RenderPass    { get; init; }
}
