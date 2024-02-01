using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.KHR;
using ThirdParty.Vulkan.Native;
using ThirdParty.Vulkan.Native.KHR;

namespace ThirdParty.Vulkan.Extensions.KHR;

public unsafe class Win32SurfaceExtension : IInstanceExtension<Win32SurfaceExtension>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface);

    public static string Name { get; } = "VK_KHR_win32_surface";

    private readonly Instance         instance;
    private readonly SurfaceExtension surfaceExtension;

    private readonly VkCreateWin32SurfaceKHR vkCreateWin32SurfaceKHR;

    internal Win32SurfaceExtension(Instance instance)
    {
        this.instance = instance;

        if (!this.instance.TryGetExtension(out this.surfaceExtension!))
        {
            throw new InvalidOperationException($"Failed to load required extension: {SurfaceExtension.Name}");
        }

        this.vkCreateWin32SurfaceKHR = instance.GetProcAddr<VkCreateWin32SurfaceKHR>(nameof(this.vkCreateWin32SurfaceKHR));
    }

    static Win32SurfaceExtension IInstanceExtension<Win32SurfaceExtension>.Create(Instance instance) =>
        new(instance);

    public Surface CreateSurface(Win32Surface.CreateInfo createInfo)
    {
        VkSurfaceKHR surfaceKHR;

        this.vkCreateWin32SurfaceKHR.Invoke(this.instance, createInfo, this.instance.Allocator, &surfaceKHR);

        return new(surfaceKHR, this.surfaceExtension);
    }
}
