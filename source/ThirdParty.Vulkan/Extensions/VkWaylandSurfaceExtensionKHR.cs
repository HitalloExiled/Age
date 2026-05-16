using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Interfaces;

using static Age.Core.PointerHelper;

namespace ThirdParty.Vulkan.Extensions;

public sealed unsafe class VkWaylandSurfaceExtensionKHR : IInstanceExtension<VkWaylandSurfaceExtensionKHR>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateWin32SurfaceKHR(
        VkHandle<VkInstance>           instance,
        VkWaylandSurfaceCreateInfoKHR* pCreateInfo,
        VkAllocationCallbacks*         pAllocator,
        VkHandle<VkSurfaceKHR>*        pSurface
    );

    public static string Name { get; } = "VK_KHR_wayland_surface";

    private readonly VkInstance         instance;
    private readonly VkSurfaceExtensionKHR surfaceExtension;

    private readonly VkCreateWin32SurfaceKHR vkCreateWaylandSurfaceKHR;

    internal VkWaylandSurfaceExtensionKHR(VkInstance instance)
    {
        this.instance = instance;

        if (!this.instance.TryGetExtension(out this.surfaceExtension!))
        {
            throw new InvalidOperationException($"Failed to load required extension: {VkSurfaceExtensionKHR.Name}");
        }

        this.vkCreateWaylandSurfaceKHR = instance.GetProcAddr<VkCreateWin32SurfaceKHR>(nameof(this.vkCreateWaylandSurfaceKHR));
    }

    static VkWaylandSurfaceExtensionKHR IInstanceExtension<VkWaylandSurfaceExtensionKHR>.Create(VkInstance instance) =>
        new(instance);

    public VkSurfaceKHR CreateSurface(in VkWaylandSurfaceCreateInfoKHR createInfo)
    {
        VkHandle<VkSurfaceKHR> surfaceKHR;

        fixed (VkAllocationCallbacks*         pAllocator  = &this.instance.Allocator)
        fixed (VkWaylandSurfaceCreateInfoKHR* pCreateInfo = &createInfo)
        {
            VkException.Check(this.vkCreateWaylandSurfaceKHR.Invoke(this.instance.Handle, pCreateInfo, NullIfDefault(pAllocator), &surfaceKHR));
        }

        return new(surfaceKHR, this.surfaceExtension);
    }
}
