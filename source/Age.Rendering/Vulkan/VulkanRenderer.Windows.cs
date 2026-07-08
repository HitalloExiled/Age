#if WINDOWS
using Age.Numerics;
using Age.Rendering.Resources;

namespace Age.Rendering.Vulkan;

public sealed partial class VulkanRenderer
{
    public Surface CreateSurface(nint handle, Size<uint> clientSize) =>
        this.Context.CreateSurface(handle, clientSize);
}
#endif
