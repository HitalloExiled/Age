using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkExtent2D.html">VkExtent2D</see>
/// </summary>
public unsafe record Extent2D : NativeReference<VkExtent2D>
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

    internal Extent2D(in VkExtent2D value) : base(value) { }
    public Extent2D() : base() { }
}
