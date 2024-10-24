using Age.Core;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public class Swapchain : Disposable
{
    public required VkExtent2D        Extent     { get; init; }
    public required VkFormat          Format     { get; init; }
    public required VkImage[]         Images     { get; init; }
    public required VkSwapchainKHR    Value      { get; init; }
    public required VkImageUsageFlags ImageUsage { get; init; }

    protected override void Disposed(bool disposing)
    {
        if (disposing)
        {
            this.Value.Dispose();
        }
    }
}
