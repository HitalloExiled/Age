
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public record Allocation : Disposable
{
    public required VkDeviceMemory Memory     { get; init; }
    public required uint           Memorytype { get; init; }
    public required ulong          Offset     { get; init; }
    public required ulong          Size       { get; init; }

    protected override void OnDispose() =>
        this.Memory.Dispose();
}