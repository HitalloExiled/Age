using Age.Vulkan.Native.Types;

namespace Age.Rendering.Vulkan;

public record Allocation
{
    public required VkDeviceMemory Memory     { get; init; }
    public required uint           Memorytype { get; init; }
    public required VkDeviceSize   Offset     { get; init; }
    public required VkDeviceSize   Size       { get; init; }
}
