#if LINUX
using System.Diagnostics.CodeAnalysis;
using Age.Numerics;
using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Extensions;

namespace Age.Rendering.Vulkan;

internal partial class VulkanContext : IDisposable
{
    private readonly string[] platformExtensions = [VkWaylandSurfaceExtensionKHR.Name];

    private VkWaylandSurfaceExtensionKHR waylandSurfaceExtension;

    [MemberNotNull(nameof(waylandSurfaceExtension))]
    public void PlatformInitialize() =>
        this.waylandSurfaceExtension = this.instance.GetExtension<VkWaylandSurfaceExtensionKHR>();

    public Surface CreateSurface(nint display, nint surface, Size<uint> size)
    {
        var createInfo = new VkWaylandSurfaceCreateInfoKHR
        {
            Display = display,
            Surface = surface,
        };

        var vksurface = this.waylandSurfaceExtension.CreateSurface(createInfo);

        return this.CreateSurface(vksurface, size);
    }
}
#endif
