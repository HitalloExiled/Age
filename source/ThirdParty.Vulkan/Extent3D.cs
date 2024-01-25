using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkExtent3D.html">VkExtent3D</see>
/// </summary>
public unsafe record Extent3D : NativeReference<VkExtent3D>
{
    public uint Width
    {
        get => this.PNative->width;
        init => this.PNative->width = value;
    }

    public uint Height
    {
        get => this.PNative->height;
        init => this.PNative->height = value;
    }

    public uint Depth
    {
        get => this.PNative->depth;
        init => this.PNative->depth = value;
    }

    internal Extent3D(in VkExtent3D value) : base(value) { }
    public Extent3D() : base() { }
}
