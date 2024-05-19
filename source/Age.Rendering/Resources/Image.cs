using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public record Image : Disposable
{
    public required Allocation Allocation { get; init; }
    public required VkExtent3D Extent     { get; init; }
    public required VkImage    Value      { get; init; }

    protected override void OnDispose()
    {
        this.Allocation.Dispose();
        this.Value.Dispose();
    }
}
