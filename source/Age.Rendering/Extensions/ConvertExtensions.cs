using Age.Numerics;
using ThirdParty.Vulkan;

namespace Age.Rendering.Extensions;

public static class ConvertExtensions
{
    public static Size<uint> ToSize(this VkExtent2D extent) =>
        new(extent.Width, extent.Height);

    public static Size<uint> ToSize(this VkExtent3D extent) =>
        new(extent.Width, extent.Height);

    public static VkExtent2D ToExtent2D(this Size<uint> size) =>
        new() { Width = size.Width, Height = size.Height };

    public static VkExtent3D ToExtent3D(this Size<uint> size, uint depth = 1) =>
        new() { Width = size.Width, Height = size.Height, Depth = depth };

    public static VkExtent3D ToExtent3D(this VkExtent2D extent, uint depth = 1) =>
        new() { Width = extent.Width, Height = extent.Height, Depth = depth };
}
