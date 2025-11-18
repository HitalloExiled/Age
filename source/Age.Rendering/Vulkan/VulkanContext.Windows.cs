#if !Windows
#define Windows
#endif

#if Windows
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Age.Numerics;
using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Extensions;

namespace Age.Rendering.Vulkan;

internal partial class VulkanContext : IDisposable
{
    private readonly string[] platformExtensions = [VkWin32SurfaceExtensionKHR.Name];

    private VkWin32SurfaceExtensionKHR win32surfaceExtension;

    [MemberNotNull(nameof(win32surfaceExtension))]
    public void PlatformInitialize() =>
        this.win32surfaceExtension = this.instance.GetExtension<VkWin32SurfaceExtensionKHR>();

    public Surface CreateSurface(nint hwnd, Size<uint> size)
    {
        var createInfo = new VkWin32SurfaceCreateInfoKHR
        {
            Hinstance = Process.GetCurrentProcess().Handle,
            Hwnd      = hwnd,
        };

        var surface = this.win32surfaceExtension.CreateSurface(createInfo);

        return this.CreateSurface(surface, size);
    }
}
#endif
