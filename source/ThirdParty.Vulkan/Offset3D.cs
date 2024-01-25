using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkOffset3D.html">VkOffset3D</see>
/// </summary>
public unsafe record Offset3D : NativeReference<VkOffset3D>
{
    public int X
    {
        get => this.PNative->x;
        init => this.PNative->x = value;
    }

    public int Y
    {
        get => this.PNative->y;
        init => this.PNative->y = value;
    }

    public int Z
    {
        get => this.PNative->z;
        init => this.PNative->z = value;
    }

    internal Offset3D(in VkOffset3D value) : base(value) { }
    public Offset3D() : base() { }
}
