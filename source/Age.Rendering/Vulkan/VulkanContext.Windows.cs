#if !Windows
#define Windows
#endif

#if Windows
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Age.Numerics;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Extensions.KHR;
using Age.Vulkan.Native.Types.KHR;

namespace Age.Rendering.Vulkan;

public unsafe partial class VulkanContext : IDisposable
{
    private readonly string[] platformExtensions = [VkKhrWin32SurfaceExtension.Name];

    private VkKhrWin32SurfaceExtension win32surfaceExtension;

    [MemberNotNull(nameof(win32surfaceExtension))]
    public void PlatformInitialize()
    {
        if (!this.vk.TryGetInstanceExtension(this.instance, out this.win32surfaceExtension!))
        {
            throw new Exception($"Cannot found required extension {VkKhrWin32SurfaceExtension.Name}");
        }
    }

    private VkSurfaceKHR CreateSurface(nint hwnd)
    {
        var surfaceCreateInfo = new VkWin32SurfaceCreateInfoKHR
        {
            hinstance = Process.GetCurrentProcess().Handle,
            hwnd      = hwnd,
        };

        return this.win32surfaceExtension.CreateWin32Surface(this.instance, surfaceCreateInfo, default, out var surface) != VkResult.VK_SUCCESS
            ? throw new Exception($"Failed to create surface")
            : surface;
    }

    public SurfaceContext CreateSurfaceContext(nint hwnd, Size<uint> size)
    {
        var surface = this.CreateSurface(hwnd);

        return this.CreateSurfaceContext(surface, size);
    }
}
#endif
