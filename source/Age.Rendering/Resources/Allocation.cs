
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public sealed class Allocation : Resource
{
    public required ulong          Alignment  { get; init; }
    public required VkDeviceMemory Memory     { get; init; }
    public required uint           Memorytype { get; init; }
    public required ulong          Offset     { get; init; }
    public required ulong          Size       { get; init; }

    protected override void Disposed() =>
        this.Memory.Dispose();
}
