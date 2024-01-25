using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkRect2D.html">VkRect2D</see>
/// </summary>
public unsafe record Rect2D : NativeReference<VkRect2D>
{
    private Offset2D? offset;
    private Extent2D? extent;

    public Offset2D? Offset
    {
        get => this.offset;
        init => this.PNative->offset = this.offset = value;
    }
    public Extent2D? Extent
    {
        get => this.extent;
        init => this.PNative->extent = this.extent = value;
    }
}
