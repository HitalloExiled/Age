using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Interfaces;

using static Age.Core.PointerHelper;

namespace ThirdParty.Vulkan.Extensions;

public sealed unsafe class VkHeadlessSurfaceExtensionEXT : IInstanceExtension<VkHeadlessSurfaceExtensionEXT>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateHeadlessSurfaceEXT(
        VkHandle<VkInstance>              instance,
        VkHeadlessSurfaceCreateInfoEXT*   pCreateInfo,
        VkAllocationCallbacks*            pAllocator,
        VkHandle<VkSurfaceKHR>*           pSurface
    );

    public static string Name { get; } = "VK_EXT_headless_surface";

    private readonly VkInstance                instance;
    private readonly VkSurfaceExtensionKHR     surfaceExtension;
    private readonly VkCreateHeadlessSurfaceEXT vkCreateHeadlessSurfaceEXT;

    internal VkHeadlessSurfaceExtensionEXT(VkInstance instance)
    {
        this.instance = instance;

        if (!this.instance.TryGetExtension(out this.surfaceExtension!))
        {
            throw new InvalidOperationException($"Failed to load required extension: {VkSurfaceExtensionKHR.Name}");
        }

        this.vkCreateHeadlessSurfaceEXT = instance.GetProcAddr<VkCreateHeadlessSurfaceEXT>(nameof(this.vkCreateHeadlessSurfaceEXT));
    }

    static VkHeadlessSurfaceExtensionEXT IInstanceExtension<VkHeadlessSurfaceExtensionEXT>.Create(VkInstance instance) =>
        new(instance);

    public VkSurfaceKHR CreateSurface(in VkHeadlessSurfaceCreateInfoEXT createInfo)
    {
        VkHandle<VkSurfaceKHR> surfaceKHR;

        fixed (VkAllocationCallbacks*          pAllocator  = &this.instance.Allocator)
        fixed (VkHeadlessSurfaceCreateInfoEXT* pCreateInfo = &createInfo)
        {
            VkException.Check(this.vkCreateHeadlessSurfaceEXT.Invoke(this.instance.Handle, pCreateInfo, NullIfDefault(pAllocator), &surfaceKHR));
        }

        return new(surfaceKHR, this.surfaceExtension);
    }
}
