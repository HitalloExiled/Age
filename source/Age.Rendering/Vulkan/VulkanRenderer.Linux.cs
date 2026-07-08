#if LINUX
using Age.Numerics;
using Age.Rendering.Resources;

namespace Age.Rendering.Vulkan;

public sealed partial class VulkanRenderer
{
    public Surface CreateSurface(nint display, nint surface, Size<uint> clientSize) =>
        this.Context.CreateSurface(display, surface, clientSize);
}
#endif
