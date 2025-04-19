using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Interfaces;

using static Age.Core.PointerHelper;

namespace ThirdParty.Vulkan.Extensions;

public sealed unsafe class VkWin32SurfaceExtensionKHR : IInstanceExtension<VkWin32SurfaceExtensionKHR>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateWin32SurfaceKHR(VkHandle<VkInstance> instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkSurfaceKHR>* pSurface);

    public static string Name { get; } = "VK_KHR_win32_surface";

    private readonly VkInstance         instance;
    private readonly VkSurfaceExtensionKHR surfaceExtension;

    private readonly VkCreateWin32SurfaceKHR vkCreateWin32SurfaceKHR;

    internal VkWin32SurfaceExtensionKHR(VkInstance instance)
    {
        this.instance = instance;

        if (!this.instance.TryGetExtension(out this.surfaceExtension!))
        {
            throw new InvalidOperationException($"Failed to load required extension: {VkSurfaceExtensionKHR.Name}");
        }

        this.vkCreateWin32SurfaceKHR = instance.GetProcAddr<VkCreateWin32SurfaceKHR>(nameof(this.vkCreateWin32SurfaceKHR));
    }

    static VkWin32SurfaceExtensionKHR IInstanceExtension<VkWin32SurfaceExtensionKHR>.Create(VkInstance instance) =>
        new(instance);

    public VkSurfaceKHR CreateSurface(in VkWin32SurfaceCreateInfoKHR createInfo)
    {
        VkHandle<VkSurfaceKHR> surfaceKHR;

        fixed (VkAllocationCallbacks*       pAllocator  = &this.instance.Allocator)
        fixed (VkWin32SurfaceCreateInfoKHR* pCreateInfo = &createInfo)
        {
            VkException.Check(this.vkCreateWin32SurfaceKHR.Invoke(this.instance.Handle, pCreateInfo, NullIfDefault(pAllocator), &surfaceKHR));
        }

        return new(surfaceKHR, this.surfaceExtension);
    }
}
