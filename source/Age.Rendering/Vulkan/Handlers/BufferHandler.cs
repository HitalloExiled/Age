using Age.Vulkan.Native.Flags;
using Age.Vulkan.Native.Types;

namespace Age.Rendering.Vulkan.Handlers;

public record BufferHandler
{
    public required Allocation         Allocation { get; init; }
    public required VkBuffer           Handler    { get; init; }
    public required VkBufferUsageFlags Usage      { get; init; }
}
