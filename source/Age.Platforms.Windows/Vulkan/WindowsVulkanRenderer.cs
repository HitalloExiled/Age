using System.Diagnostics;
using Age.Numerics;
using Age.Platforms.Windows.Native.Types;
using Age.Rendering.Vulkan;
using Age.Rendering.Vulkan.Handlers;
using Age.Vulkan.Native;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Extensions.KHR;
using Age.Vulkan.Native.Types.KHR;

namespace Age.Platforms.Windows.Vulkan;

public class WindowsVulkanContext(Vk vk) : VulkanContext(vk)
{
    protected override string[] PlatformExtensions => [VkKhrWin32SurfaceExtension.Name];

    private VkSurfaceKHR CreateSurface(HWND hwnd)
    {
        if (!this.Vk.TryGetInstanceExtension<VkKhrWin32SurfaceExtension>(this.Instance, out var vkKhrWin32SurfaceExtension))
        {
            throw new Exception($"Cannot found required extension {VkKhrWin32SurfaceExtension.Name}");
        }

        var surfaceCreateInfo = new VkWin32SurfaceCreateInfoKHR
        {
            hinstance = Process.GetCurrentProcess().Handle,
            hwnd      = hwnd,
        };

        return vkKhrWin32SurfaceExtension.CreateWin32Surface(this.Instance, surfaceCreateInfo, default, out var surface) != VkResult.VK_SUCCESS
            ? throw new Exception($"Failed to create surface")
            : surface;
    }

    public WindowContext CreateWindow(HWND hwnd, Size<uint> size)
    {
        var surface = this.CreateSurface(hwnd);

        return this.CreateWindow(surface, size);
    }
}
