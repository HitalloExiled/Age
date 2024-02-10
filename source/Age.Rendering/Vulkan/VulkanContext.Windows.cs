#if !Windows
#define Windows
#endif

#if Windows
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Age.Numerics;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Extensions;

namespace Age.Rendering.Vulkan;

public unsafe partial class VulkanContext : IDisposable
{
    private readonly string[] platformExtensions = [VkWin32SurfaceExtensionKHR.Name];

    private VkWin32SurfaceExtensionKHR win32surfaceExtension;

    [MemberNotNull(nameof(win32surfaceExtension))]
    public void PlatformInitialize()
    {
        this.win32surfaceExtension = instance.GetExtension<VkWin32SurfaceExtensionKHR>();
    }

    private VkSurfaceKHR CreateSurface(nint hwnd)
    {
        var createInfo = new VkWin32SurfaceCreateInfoKHR
        {
            Hinstance = Process.GetCurrentProcess().Handle,
            Hwnd      = hwnd,
        };

        return this.win32surfaceExtension.CreateSurface(createInfo);
    }

    public SurfaceContext CreateSurfaceContext(nint hwnd, Size<uint> size)
    {
        var surface = this.CreateSurface(hwnd);

        return this.CreateSurfaceContext(surface, size);
    }
}
#endif
