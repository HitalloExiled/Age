
using ThirdParty.Vulkan;

namespace Age.Rendering.Vulkan;

public record Allocation
{
    public required VkDeviceMemory Memory     { get; init; }
    public required uint           Memorytype { get; init; }
    public required ulong          Offset     { get; init; }
    public required ulong          Size       { get; init; }
}
