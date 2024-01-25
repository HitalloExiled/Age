using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkOffset2D" />
public unsafe record Offset2D : NativeReference<VkOffset2D>
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

    internal Offset2D(in VkOffset2D value) : base(value) { }
    public Offset2D() : base() { }
}
