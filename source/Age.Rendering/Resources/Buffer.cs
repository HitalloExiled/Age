using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public record Buffer : Disposable
{
    public required Allocation         Allocation { get; init; }
    public required VkBuffer           Value      { get; init; }
    public required VkBufferUsageFlags Usage      { get; init; }

    protected override void OnDispose()
    {
        this.Allocation.Dispose();
        this.Value.Dispose();
    }
}
