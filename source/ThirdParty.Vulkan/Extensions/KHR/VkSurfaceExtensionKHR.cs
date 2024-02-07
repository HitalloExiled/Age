using System.Runtime.InteropServices;
using Age.Core.Interop;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Enums.KHR;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.KHR;
using ThirdParty.Vulkan.Native;
using ThirdParty.Vulkan.Native.KHR;

namespace ThirdParty.Vulkan.Extensions.KHR;

public unsafe class VkSurfaceExtensionKHR : IInstanceExtension<VkSurfaceExtensionKHR>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroySurfaceKHR(VkHandle<VkInstance> instance, VkHandle<VkSurfaceKHR> surface, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkGetPhysicalDeviceSurfaceCapabilitiesKHR(VkHandle<VkPhysicalDevice> physicalDevice, VkHandle<VkSurfaceKHR> surface, VkSurfaceCapabilitiesKHR* pSurfaceCapabilities);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkGetPhysicalDeviceSurfaceFormatsKHR(VkHandle<VkPhysicalDevice> physicalDevice, VkHandle<VkSurfaceKHR> surface, uint* pSurfaceFormatCount, VkSurfaceFormatKHR* pSurfaceFormats);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkGetPhysicalDeviceSurfacePresentModesKHR(VkHandle<VkPhysicalDevice> physicalDevice, VkHandle<VkSurfaceKHR> surface, uint* pPresentModeCount, VkPresentModeKHR* pPresentModes);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkGetPhysicalDeviceSurfaceSupportKHR(VkHandle<VkPhysicalDevice> physicalDevice, uint queueFamilyIndex, VkHandle<VkSurfaceKHR> surface, VkBool32* pSupported);

    private readonly VkInstance                                instance;
    private readonly VkDestroySurfaceKHR                       vkDestroySurfaceKHR;
    private readonly VkGetPhysicalDeviceSurfaceCapabilitiesKHR vkGetPhysicalDeviceSurfaceCapabilitiesKHR;
    private readonly VkGetPhysicalDeviceSurfaceFormatsKHR      vkGetPhysicalDeviceSurfaceFormatsKHR;
    private readonly VkGetPhysicalDeviceSurfacePresentModesKHR vkGetPhysicalDeviceSurfacePresentModesKHR;
    private readonly VkGetPhysicalDeviceSurfaceSupportKHR      vkGetPhysicalDeviceSurfaceSupportKHR;

    public static string Name { get; } = "VK_KHR_surface";

    internal VkSurfaceExtensionKHR(VkInstance instance)
    {
        this.instance = instance;

        this.vkDestroySurfaceKHR                       = instance.GetProcAddr<VkDestroySurfaceKHR>("vkDestroySurfaceKHR");
        this.vkGetPhysicalDeviceSurfaceCapabilitiesKHR = instance.GetProcAddr<VkGetPhysicalDeviceSurfaceCapabilitiesKHR>("vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
        this.vkGetPhysicalDeviceSurfaceFormatsKHR      = instance.GetProcAddr<VkGetPhysicalDeviceSurfaceFormatsKHR>("vkGetPhysicalDeviceSurfaceFormatsKHR");
        this.vkGetPhysicalDeviceSurfacePresentModesKHR = instance.GetProcAddr<VkGetPhysicalDeviceSurfacePresentModesKHR>("vkGetPhysicalDeviceSurfacePresentModesKHR");
        this.vkGetPhysicalDeviceSurfaceSupportKHR      = instance.GetProcAddr<VkGetPhysicalDeviceSurfaceSupportKHR>("vkGetPhysicalDeviceSurfaceSupportKHR");
    }

    static VkSurfaceExtensionKHR IInstanceExtension<VkSurfaceExtensionKHR>.Create(VkInstance instance) =>
        new(instance);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroySurfaceKHR.html">vkDestroySurfaceKHR</see>
    /// </summary>
    public void DestroySurface(VkSurfaceKHR surface)
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.instance.Allocator)
        {
            this.vkDestroySurfaceKHR.Invoke(this.instance.Handle, surface.Handle, PointerHelper.NullIfDefault(this.instance.Allocator, pAllocator));
        }
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceSurfaceCapabilitiesKHR.html">vkGetPhysicalDeviceSurfaceCapabilitiesKHR</see>
    /// </summary>
    public void GetPhysicalDeviceSurfaceCapabilities(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, out VkSurfaceCapabilitiesKHR surfaceCapabilities)
    {
        fixed (VkSurfaceCapabilitiesKHR* pSurfaceCapabilities = &surfaceCapabilities)
        {
            VkException.Check(this.vkGetPhysicalDeviceSurfaceCapabilitiesKHR.Invoke(physicalDevice.Handle, surface.Handle, pSurfaceCapabilities));
        }
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceSurfaceFormatsKHR.html">vkGetPhysicalDeviceSurfaceFormatsKHR</see>
    /// </summary>
    public VkSurfaceFormatKHR[] GetPhysicalDeviceSurfaceFormats(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface)
    {
        uint surfaceFormatCount;

        VkException.Check(this.vkGetPhysicalDeviceSurfaceFormatsKHR.Invoke(physicalDevice.Handle, surface.Handle, &surfaceFormatCount, null));

        var surfaceFormats = new VkSurfaceFormatKHR[surfaceFormatCount];

        fixed (VkSurfaceFormatKHR* pSurfaceFormats = surfaceFormats)
        {
            VkException.Check(this.vkGetPhysicalDeviceSurfaceFormatsKHR.Invoke(physicalDevice.Handle, surface.Handle, &surfaceFormatCount, pSurfaceFormats));
        }

        return surfaceFormats;
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceSurfacePresentModesKHR.html">vkGetPhysicalDeviceSurfacePresentModesKHR</see>
    /// </summary>
    public VkPresentModeKHR[] GetPhysicalDeviceSurfacePresentModes(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface)
    {
        uint presentModeCount;

        VkException.Check(this.vkGetPhysicalDeviceSurfacePresentModesKHR.Invoke(physicalDevice.Handle, surface.Handle, &presentModeCount, null));

        var presentModes = new VkPresentModeKHR[presentModeCount];

        fixed (VkPresentModeKHR* pPresentModes = presentModes)
        {
            VkException.Check(this.vkGetPhysicalDeviceSurfacePresentModesKHR.Invoke(physicalDevice.Handle, surface.Handle, &presentModeCount, pPresentModes));
        }

        return presentModes;
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceSurfaceSupportKHR.html">vkGetPhysicalDeviceSurfaceSupportKHR</see>
    /// </summary>
    public bool GetPhysicalDeviceSurfaceSupport(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, VkSurfaceKHR surface)
    {
        VkBool32 supported;

        VkException.Check(this.vkGetPhysicalDeviceSurfaceSupportKHR.Invoke(physicalDevice.Handle, queueFamilyIndex, surface.Handle, &supported));

        return supported;
    }
}
