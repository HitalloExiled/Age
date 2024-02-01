using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums.KHR;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.KHR;
using ThirdParty.Vulkan.Native;
using ThirdParty.Vulkan.Native.KHR;

namespace ThirdParty.Vulkan.Extensions.KHR;

public unsafe class SurfaceExtension : IInstanceExtension<SurfaceExtension>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroySurfaceKHR(VkInstance instance, VkSurfaceKHR surface, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkGetPhysicalDeviceSurfaceCapabilitiesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, VkSurfaceCapabilitiesKHR* pSurfaceCapabilities);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkGetPhysicalDeviceSurfaceFormatsKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pSurfaceFormatCount, VkSurfaceFormatKHR* pSurfaceFormats);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkGetPhysicalDeviceSurfacePresentModesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pPresentModeCount, VkPresentModeKHR* pPresentModes);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkGetPhysicalDeviceSurfaceSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, VkSurfaceKHR surface, VkBool32* pSupported);

    private readonly Instance                                  instance;
    private readonly VkDestroySurfaceKHR                       vkDestroySurfaceKHR;
    private readonly VkGetPhysicalDeviceSurfaceCapabilitiesKHR vkGetPhysicalDeviceSurfaceCapabilitiesKHR;
    private readonly VkGetPhysicalDeviceSurfaceFormatsKHR      vkGetPhysicalDeviceSurfaceFormatsKHR;
    private readonly VkGetPhysicalDeviceSurfacePresentModesKHR vkGetPhysicalDeviceSurfacePresentModesKHR;
    private readonly VkGetPhysicalDeviceSurfaceSupportKHR      vkGetPhysicalDeviceSurfaceSupportKHR;

    public static string Name { get; } = "VK_KHR_surface";

    internal SurfaceExtension(Instance instance)
    {
        this.instance = instance;

        this.vkDestroySurfaceKHR                       = instance.GetProcAddr<VkDestroySurfaceKHR>("vkDestroySurfaceKHR");
        this.vkGetPhysicalDeviceSurfaceCapabilitiesKHR = instance.GetProcAddr<VkGetPhysicalDeviceSurfaceCapabilitiesKHR>("vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
        this.vkGetPhysicalDeviceSurfaceFormatsKHR      = instance.GetProcAddr<VkGetPhysicalDeviceSurfaceFormatsKHR>("vkGetPhysicalDeviceSurfaceFormatsKHR");
        this.vkGetPhysicalDeviceSurfacePresentModesKHR = instance.GetProcAddr<VkGetPhysicalDeviceSurfacePresentModesKHR>("vkGetPhysicalDeviceSurfacePresentModesKHR");
        this.vkGetPhysicalDeviceSurfaceSupportKHR      = instance.GetProcAddr<VkGetPhysicalDeviceSurfaceSupportKHR>("vkGetPhysicalDeviceSurfaceSupportKHR");
    }

    static SurfaceExtension IInstanceExtension<SurfaceExtension>.Create(Instance instance) =>
        new(instance);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroySurfaceKHR.html">vkDestroySurfaceKHR</see>
    /// </summary>
    public void DestroySurface(Surface surface) =>
        this.vkDestroySurfaceKHR.Invoke(this.instance, surface, this.instance.Allocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceSurfaceCapabilitiesKHR.html">vkGetPhysicalDeviceSurfaceCapabilitiesKHR</see>
    /// </summary>
    public SurfaceCapabilities GetPhysicalDeviceSurfaceCapabilities(VkPhysicalDevice physicalDevice, Surface surface)
    {
        VkSurfaceCapabilitiesKHR surfaceCapabilities;

        VulkanException.Check(this.vkGetPhysicalDeviceSurfaceCapabilitiesKHR.Invoke(physicalDevice, surface, &surfaceCapabilities));

        return new(surfaceCapabilities);
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceSurfaceFormatsKHR.html">vkGetPhysicalDeviceSurfaceFormatsKHR</see>
    /// </summary>
    public SurfaceFormat[] GetPhysicalDeviceSurfaceFormats(PhysicalDevice physicalDevice, Surface surface)
    {
        uint surfaceFormatCount;

        VulkanException.Check(this.vkGetPhysicalDeviceSurfaceFormatsKHR.Invoke(physicalDevice, surface, &surfaceFormatCount, null));

        var vkSurfaceFormats = new VkSurfaceFormatKHR[surfaceFormatCount];

        fixed (VkSurfaceFormatKHR* pSurfaceFormats = vkSurfaceFormats)
        {
            VulkanException.Check(this.vkGetPhysicalDeviceSurfaceFormatsKHR.Invoke(physicalDevice, surface, &surfaceFormatCount, pSurfaceFormats));
        }

        var surfaceFormats = new SurfaceFormat[surfaceFormatCount];

        for (var i = 0; i < surfaceFormatCount; i++)
        {
            surfaceFormats[i] = new(vkSurfaceFormats[i]);
        }

        return surfaceFormats;
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceSurfacePresentModesKHR.html">vkGetPhysicalDeviceSurfacePresentModesKHR</see>
    /// </summary>
    public PresentMode[] GetPhysicalDeviceSurfacePresentModes(VkPhysicalDevice physicalDevice, Surface surface)
    {
        uint presentModeCount;

        VulkanException.Check(this.vkGetPhysicalDeviceSurfacePresentModesKHR.Invoke(physicalDevice, surface, &presentModeCount, null));

        var presentModes = new PresentMode[presentModeCount];

        fixed (PresentMode* pPresentModes = presentModes)
        {
            VulkanException.Check(this.vkGetPhysicalDeviceSurfacePresentModesKHR.Invoke(physicalDevice, surface, &presentModeCount, pPresentModes));
        }

        return presentModes;
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceSurfaceSupportKHR.html">vkGetPhysicalDeviceSurfaceSupportKHR</see>
    /// </summary>
    public bool GetPhysicalDeviceSurfaceSupport(PhysicalDevice physicalDevice, uint queueFamilyIndex, Surface surface)
    {
        VkBool32 supported;

        VulkanException.Check(this.vkGetPhysicalDeviceSurfaceSupportKHR.Invoke(physicalDevice, queueFamilyIndex, surface, &supported));

        return supported;
    }
}
