using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public record Swapchain : Disposable
{
    public required VkExtent2D      Extent { get; init; }
    public required VkFormat        Format { get; init; }
    public required VkImage[]       Images { get; init; }
    public required VkSwapchainKHR  Value  { get; init; }

    protected override void OnDispose() =>
        this.Value.Dispose();
}
