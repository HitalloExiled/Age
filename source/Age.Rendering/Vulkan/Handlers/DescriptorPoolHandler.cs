using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Vulkan;

public record DescriptorPoolHandler
{
    public required VkDescriptorType DescriptorType { get; init; }
    public required VkDescriptorPool Handler        { get; init; }

    public uint Usage { get; set; }
}
