using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Vulkan.Handlers;

public record BufferHandler
{
    public required Allocation         Allocation { get; init; }
    public required VkBuffer           Handler    { get; init; }
    public required VkBufferUsageFlags Usage      { get; init; }
}
