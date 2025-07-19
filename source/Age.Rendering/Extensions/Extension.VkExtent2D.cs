using Age.Numerics;
using ThirdParty.Vulkan;

namespace Age.Rendering.Extensions;

public static partial class Extension
{
    extension(in VkExtent2D extent)
    {
        public Size<uint> ToSize() =>
            new(extent.Width, extent.Height);

        public VkExtent3D ToExtent3D(uint depth = 1) =>
            new() { Width = extent.Width, Height = extent.Height, Depth = depth };
    }
}
