using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public readonly ref struct RenderTargetCreateInfo
{
    public required VkExtent3D Extent     { get; init; }
    public required VkFormat   Format     { get; init; }
    public required RenderPass RenderPass { get; init; }
}
