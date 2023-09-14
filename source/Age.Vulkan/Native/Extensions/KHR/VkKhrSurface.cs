using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Native.Extensions.KHR;

public unsafe class VkKhrSurface : IVkInstanceExtension
{
    private delegate VkResult VkGetPhysicalDeviceSurfaceSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, VkSurfaceKHR surface, VkBool32* pSupported);
    private delegate void VkDestroySurfaceKHR(VkInstance instance, VkSurfaceKHR surface, VkAllocationCallbacks* pAllocator);

    public static string Name => "VK_KHR_surface";

    private readonly VkGetPhysicalDeviceSurfaceSupportKHR vkGetPhysicalDeviceSurfaceSupportKHR;
    private readonly VkDestroySurfaceKHR                  vkDestroySurfaceKHR;

    public VkKhrSurface(Vk vk, VkInstance instance)
    {
        this.vkGetPhysicalDeviceSurfaceSupportKHR = vk.GetInstanceProcAddr<VkGetPhysicalDeviceSurfaceSupportKHR>(Name, instance, "vkGetPhysicalDeviceSurfaceSupportKHR");
        this.vkDestroySurfaceKHR                  = vk.GetInstanceProcAddr<VkDestroySurfaceKHR>(Name, instance, "vkDestroySurfaceKHR");
    }

    public static IVkInstanceExtension Create(Vk vk, VkInstance instance) =>
        new VkKhrSurface(vk, instance);

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
}
