using Age.Numerics;
using ThirdParty.Vulkan;

namespace Age.Rendering.Extensions;

public static partial class Extension
{
    extension(in Size<uint> size)
    {
        public VkExtent2D ToExtent2D() =>
            new() { Width = size.Width, Height = size.Height };

        public VkExtent3D ToExtent3D(uint depth = 1) =>
            new() { Width = size.Width, Height = size.Height, Depth = depth };
    }
}
