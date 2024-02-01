

using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Enums.KHR;
using ThirdParty.Vulkan.Native.KHR;

namespace ThirdParty.Vulkan.KHR;

#pragma warning disable IDE0001

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSurfaceFormatKHR.html">VkSurfaceFormatKHR</see>
/// </summary>
public unsafe record SurfaceFormat : NativeReference<VkSurfaceFormatKHR>
{
    public Format Format
    {
        get => this.PNative->format;
        init => this.PNative->format = value;
    }

    public ColorSpace ColorSpace
    {
        get => this.PNative->colorSpace;
        init => this.PNative->colorSpace = value;
    }

    internal SurfaceFormat(in VkSurfaceFormatKHR value) : base(value) { }
}
