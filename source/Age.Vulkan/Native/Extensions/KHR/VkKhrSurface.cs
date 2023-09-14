using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Extensions.KHR.Types;
using Age.Vulkan.Native.Types;

namespace Age.Vulkan.Native.Extensions.KHR;

public unsafe class VkKhrSurface : IVkInstanceExtension
{
    private delegate void VkDestroySurfaceKHR(VkInstance instance, VkSurfaceKHR surface, VkAllocationCallbacks* pAllocator);
    private delegate VkResult VkGetPhysicalDeviceSurfaceCapabilitiesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, VkSurfaceCapabilitiesKHR* pSurfaceCapabilities);
    private delegate VkResult VkGetPhysicalDeviceSurfaceFormatsKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pSurfaceFormatCount, VkSurfaceFormatKHR* pSurfaceFormats);
    private delegate VkResult VkGetPhysicalDeviceSurfacePresentModesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pPresentModeCount, VkPresentModeKHR* pPresentModes);
    private delegate VkResult VkGetPhysicalDeviceSurfaceSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, VkSurfaceKHR surface, VkBool32* pSupported);

    public static string Name { get; } = "VK_KHR_surface";

    private readonly VkDestroySurfaceKHR                       vkDestroySurfaceKHR;
    private readonly VkGetPhysicalDeviceSurfaceCapabilitiesKHR vkGetPhysicalDeviceSurfaceCapabilitiesKHR;
    private readonly VkGetPhysicalDeviceSurfaceFormatsKHR      vkGetPhysicalDeviceSurfaceFormatsKHR;
    private readonly VkGetPhysicalDeviceSurfacePresentModesKHR vkGetPhysicalDeviceSurfacePresentModesKHR;
    private readonly VkGetPhysicalDeviceSurfaceSupportKHR      vkGetPhysicalDeviceSurfaceSupportKHR;

    public VkKhrSurface(Vk vk, VkInstance instance)
    {
        this.vkDestroySurfaceKHR                       = vk.GetInstanceProcAddr<VkDestroySurfaceKHR>(Name, instance, "vkDestroySurfaceKHR");
        this.vkGetPhysicalDeviceSurfaceCapabilitiesKHR = vk.GetInstanceProcAddr<VkGetPhysicalDeviceSurfaceCapabilitiesKHR>(Name, instance, "vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
        this.vkGetPhysicalDeviceSurfaceFormatsKHR      = vk.GetInstanceProcAddr<VkGetPhysicalDeviceSurfaceFormatsKHR>(Name, instance, "vkGetPhysicalDeviceSurfaceFormatsKHR");
        this.vkGetPhysicalDeviceSurfacePresentModesKHR = vk.GetInstanceProcAddr<VkGetPhysicalDeviceSurfacePresentModesKHR>(Name, instance, "vkGetPhysicalDeviceSurfacePresentModesKHR");
        this.vkGetPhysicalDeviceSurfaceSupportKHR      = vk.GetInstanceProcAddr<VkGetPhysicalDeviceSurfaceSupportKHR>(Name, instance, "vkGetPhysicalDeviceSurfaceSupportKHR");
    }

    public static IVkInstanceExtension Create(Vk vk, VkInstance instance) =>
        new VkKhrSurface(vk, instance);

    /// <summary>
    /// <para>Destroy a <see cref="VkSurfaceKHR"/> object.</para>
    /// <para>Destroying a <see cref="VkSurfaceKHR"/> merely severs the connection between Vulkan and the native surface, and does not imply destroying the native surface, closing a window, or similar behavior.</para>
    /// </summary>
    /// <param name="instance">The instance used to create the surface.</param>
    /// <param name="surface">The surface to destroy.</param>
    /// <param name="pAllocator">The allocator used for host memory allocated for the surface object when there is no more specific allocator available (see <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see>).</param>
    public void DestroySurface(VkInstance instance, VkSurfaceKHR surface, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroySurfaceKHR.Invoke(instance, surface, pAllocator);

    public void DestroySurface(VkInstance instance, VkSurfaceKHR surface, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroySurfaceKHR.Invoke(instance, surface, allocator.Equals(default(VkAllocationCallbacks)) ? null : pAllocator);
        }
    }

    /// <summary>
    /// Query if presentation is supported.
    /// </summary>
    /// <param name="physicalDevice">The physical device.</param>
    /// <param name="queueFamilyIndex">The queue family.</param>
    /// <param name="surface">The surface.</param>
    /// <param name="pSupported">A pointer to a <see cref="VkBool32"/>, which is set to true to indicate support, and false otherwise.</param>
    public VkResult GetPhysicalDeviceSurfaceSupport(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, VkSurfaceKHR surface, VkBool32* pSupported) =>
        this.vkGetPhysicalDeviceSurfaceSupportKHR.Invoke(physicalDevice, queueFamilyIndex, surface, pSupported);

    public VkResult GetPhysicalDeviceSurfaceSupport(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, VkSurfaceKHR surface, out VkBool32 supported)
    {
        fixed (VkBool32* pSupported = &supported)
        {
            return this.vkGetPhysicalDeviceSurfaceSupportKHR.Invoke(physicalDevice, queueFamilyIndex, surface, pSupported);
        }
    }

    /// <summary>
    /// <para>Query color formats supported by surface.</para>
    /// <para>If pSurfaceFormats is NULL, then the number of format pairs supported for the given surface is returned in pSurfaceFormatCount. Otherwise, pSurfaceFormatCount must point to a variable set by the user to the number of elements in the pSurfaceFormats array, and on return the variable is overwritten with the number of structures actually written to pSurfaceFormats. If the value of pSurfaceFormatCount is less than the number of format pairs supported, at most pSurfaceFormatCount structures will be written, and VK_INCOMPLETE will be returned instead of <see cref="VkResult.VK_SUCCESS"/>, to indicate that not all the available format pairs were returned.</para>
    /// <para>The number of format pairs supported must be greater than or equal to 1. pSurfaceFormats must not contain an entry whose value for format is VK_FORMAT_UNDEFINED.</para>
    /// <para>If pSurfaceFormats includes an entry whose value for colorSpace is <see cref="VkColorSpace.VK_COLOR_SPACE_SRGB_NONLINEAR_KHR"/> and whose value for format is a UNORM (or SRGB) format and the corresponding SRGB (or UNORM) format is a color renderable format for <see cref="VkImageTiling.VK_IMAGE_TILING_OPTIMAL"/>, then pSurfaceFormats must also contain an entry with the same value for colorSpace and format equal to the corresponding SRGB (or UNORM) format.</para>
    /// <para>If the VK_GOOGLE_surfaceless_query extension is enabled, the values returned in pSurfaceFormats will be identical for every valid surface created on this physical device, and so surface can be VK_NULL_HANDLE.</para>
    /// </summary>
    /// <param name="physicalDevice"></param>
    /// <param name="surface"></param>
    /// <param name="pSurfaceFormatCount"></param>
    /// <param name="pSurfaceFormats"></param>
    /// <returns></returns>
    public VkResult GetPhysicalDeviceSurfaceFormats(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pSurfaceFormatCount, VkSurfaceFormatKHR* pSurfaceFormats) =>
        this.vkGetPhysicalDeviceSurfaceFormatsKHR.Invoke(physicalDevice, surface, pSurfaceFormatCount, pSurfaceFormats);

    public VkResult GetPhysicalDeviceSurfaceFormats(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, out uint surfaceFormatCount)
    {
        fixed (uint* pSurfaceFormatCount = &surfaceFormatCount)
        {
            return this.vkGetPhysicalDeviceSurfaceFormatsKHR.Invoke(physicalDevice, surface, pSurfaceFormatCount, null);
        }
    }

    public VkResult GetPhysicalDeviceSurfaceFormats(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, out VkSurfaceFormatKHR[] surfaceFormats)
    {
        if (this.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, out uint surfaceFormatCount) is var result and not VkResult.VK_SUCCESS)
        {
            surfaceFormats = Array.Empty<VkSurfaceFormatKHR>();

            return result;
        }

        surfaceFormats = new VkSurfaceFormatKHR[surfaceFormatCount];

        fixed (VkSurfaceFormatKHR* pSurfaceFormats = surfaceFormats)
        {
            return this.vkGetPhysicalDeviceSurfaceFormatsKHR.Invoke(physicalDevice, surface, &surfaceFormatCount, pSurfaceFormats);
        }
    }

    /// <summary>
    /// <para>Query supported presentation modes.</para>
    /// <para>If pPresentModes is NULL, then the number of presentation modes supported for the given surface is returned in pPresentModeCount. Otherwise, pPresentModeCount must point to a variable set by the user to the number of elements in the pPresentModes array, and on return the variable is overwritten with the number of values actually written to pPresentModes. If the value of pPresentModeCount is less than the number of presentation modes supported, at most pPresentModeCount values will be written, and VK_INCOMPLETE will be returned instead of VK_SUCCESS, to indicate that not all the available modes were returned.</para>
    /// <para>If the VK_GOOGLE_surfaceless_query extension is enabled and surface is VK_NULL_HANDLE, the values returned in pPresentModes will only indicate support for <see cref="VkPresentModeKHR.VK_PRESENT_MODE_FIFO_KHR"/>, <see cref="VkPresentModeKHR.VK_PRESENT_MODE_SHARED_DEMAND_REFRESH_KHR"/>, and <see cref="VkPresentModeKHR.VK_PRESENT_MODE_SHARED_CONTINUOUS_REFRESH_KHR"/>. To query support for any other present mode, a valid handle must be provided in surface.</para>
    /// </summary>
    /// <param name="physicalDevice"></param>
    /// <param name="surface"></param>
    /// <param name="pPresentModeCount"></param>
    /// <param name="pPresentModes"></param>
    public VkResult GetPhysicalDeviceSurfacePresentModes(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pPresentModeCount, VkPresentModeKHR* pPresentModes) =>
        this.vkGetPhysicalDeviceSurfacePresentModesKHR.Invoke(physicalDevice, surface, pPresentModeCount, pPresentModes);

    public VkResult GetPhysicalDeviceSurfacePresentModes(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, out uint presentModeCount)
    {
        fixed (uint* pPresentModeCount = &presentModeCount)
        {
            return this.vkGetPhysicalDeviceSurfacePresentModesKHR.Invoke(physicalDevice, surface, pPresentModeCount, null);
        }
    }

    public VkResult GetPhysicalDeviceSurfacePresentModes(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, out VkPresentModeKHR[] presentModes)
    {
        if (this.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, out uint presentModeCount) is var result and not VkResult.VK_SUCCESS)
        {
            presentModes = Array.Empty<VkPresentModeKHR>();

            return result;
        }

        presentModes = new VkPresentModeKHR[presentModeCount];

        fixed (VkPresentModeKHR* pPresentModes = presentModes.AsSpan())
        {
            return this.vkGetPhysicalDeviceSurfacePresentModesKHR.Invoke(physicalDevice, surface, &presentModeCount, pPresentModes);
        }
    }

    /// <summary>
    /// Query surface capabilities.
    /// </summary>
    /// <param name="physicalDevice">The physical device that will be associated with the swapchain to be created, as described for vkCreateSwapchainKHR.</param>
    /// <param name="surface">The surface that will be associated with the swapchain.</param>
    /// <param name="pSurfaceCapabilities">A pointer to a VkSurfaceCapabilitiesKHR structure in which the capabilities are returned.</param>
    public VkResult GetPhysicalDeviceSurfaceCapabilities(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, VkSurfaceCapabilitiesKHR* pSurfaceCapabilities) =>
        this.vkGetPhysicalDeviceSurfaceCapabilitiesKHR.Invoke(physicalDevice, surface, pSurfaceCapabilities);

    public VkResult GetPhysicalDeviceSurfaceCapabilities(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, out VkSurfaceCapabilitiesKHR surfaceCapabilities)
    {
        fixed (VkSurfaceCapabilitiesKHR* pSurfaceCapabilities = &surfaceCapabilities)
        {
            return this.vkGetPhysicalDeviceSurfaceCapabilitiesKHR.Invoke(physicalDevice, surface, pSurfaceCapabilities);
        }
    }
}
