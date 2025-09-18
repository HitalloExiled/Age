using System.Numerics;
using Age.Numerics;
using ThirdParty.Vulkan;

namespace Age.Rendering.Extensions;

public static partial class Extension
{
    extension<T>(in Extent<T> extent) where T : INumber<T>
    {
        public Size<T> ToSize() =>
            new() { Width = extent.Width, Height = extent.Height };
    }

    extension<T>(in Extent<uint> extent) where T : INumber<T>
    {
        public VkExtent3D ToVkExtent3D() =>
            new() { Width = extent.Width, Height = extent.Height };
    }
}
