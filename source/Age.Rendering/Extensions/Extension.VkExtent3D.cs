using Age.Numerics;
using ThirdParty.Vulkan;

namespace Age.Rendering.Extensions;

public static partial class Extension
{
    extension(in VkExtent3D extent)
    {
        public Size<uint> ToSize() =>
            new(extent.Width, extent.Height);

        public Extent<uint> ToExtent() =>
            new(extent.Width, extent.Height, extent.Depth);
    }
}
