using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Types;

namespace Age.Rendering.Vulkan;

public record DescriptorPoolHandler
{
    public required VkDescriptorType DescriptorType { get; init; }
    public required VkDescriptorPool Handler        { get; init; }

    public uint Usage { get; set; }
}
