using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Extensions.EXT.Types;
using Age.Vulkan.Native.Types;

namespace Age.Vulkan.Native.Extensions.EXT;

public unsafe class VkExtDebugUtils : IVkInstanceExtension
{
    private delegate VkResult VkCreateDebugUtilsMessengerEXT(VkInstance instance, VkDebugUtilsMessengerCreateInfoEXT* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDebugUtilsMessengerEXT* pMessenger);
    private delegate void VkDestroyDebugUtilsMessengerEXT(VkInstance instance, VkDebugUtilsMessengerEXT messenger, VkAllocationCallbacks* pAllocator);

    public static string Name { get; } = "VK_EXT_debug_utils";

    private readonly VkCreateDebugUtilsMessengerEXT vkCreateDebugUtilsMessengerEXT;
    private readonly VkDestroyDebugUtilsMessengerEXT vkDestroyDebugUtilsMessengerEXT;

    public VkExtDebugUtils(Vk vk, VkInstance instance)
    {
        this.vkCreateDebugUtilsMessengerEXT  = vk.GetInstanceProcAddr<VkCreateDebugUtilsMessengerEXT>(Name, instance, "vkCreateDebugUtilsMessengerEXT");
        this.vkDestroyDebugUtilsMessengerEXT = vk.GetInstanceProcAddr<VkDestroyDebugUtilsMessengerEXT>(Name, instance, "vkDestroyDebugUtilsMessengerEXT");
    }

    public static IVkInstanceExtension Create(Vk vk, VkInstance instance) =>
        new VkExtDebugUtils(vk, instance);

    /// <summary>
    /// Create a debug messenger object
    /// </summary>
    /// <param name="instance">The instance the messenger will be used with.</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkDebugUtilsMessengerCreateInfoEXT"/> structure containing the callback pointer, as well as defining conditions under which this messenger will trigger the callback.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the Memory Allocation chapter.</param>
    /// <param name="pMessenger">A pointer to a <see cref="VkDebugUtilsMessengerEXT"/> handle in which the created object is returned.</param>
    /// <remarks>The application must ensure that vkCreateDebugUtilsMessengerEXT is not executed in parallel with any Vulkan command that is also called with instance or child of instance as the dispatchable argument.</remarks>
    public VkResult CreateDebugUtilsMessenger(VkInstance instance, VkDebugUtilsMessengerCreateInfoEXT* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDebugUtilsMessengerEXT* pMessenger) =>
        this.vkCreateDebugUtilsMessengerEXT.Invoke(instance, pCreateInfo, pAllocator, pMessenger);

    public VkResult CreateDebugUtilsMessenger(VkInstance instance, in VkDebugUtilsMessengerCreateInfoEXT createInfo, in VkAllocationCallbacks allocator, out VkDebugUtilsMessengerEXT messenger)
    {
        fixed (VkDebugUtilsMessengerCreateInfoEXT* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator               = &allocator)
        fixed (VkDebugUtilsMessengerEXT* pMessenger            = &messenger)
        {
            return this.vkCreateDebugUtilsMessengerEXT.Invoke(
                instance,
                createInfo.Equals(default(VkDebugUtilsMessengerCreateInfoEXT)) ? null : pCreateInfo,
                allocator.Equals(default(VkAllocationCallbacks))               ? null : pAllocator,
                pMessenger
            );
        }
    }

    /// <summary>
    /// Destroy a debug messenger object
    /// </summary>
    /// <param name="instance">The instance where the callback was created.</param>
    /// <param name="messenger">The <see cref="VkDebugUtilsMessengerEXT"/> object to destroy. messenger is an externally synchronized object and must not be used on more than one thread at a time. This means that <see cref="DestroyDebugUtilsMessengerEXT"/> must not be called when a callback is active.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    public void DestroyDebugUtilsMessenger(VkInstance instance, VkDebugUtilsMessengerEXT messenger, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyDebugUtilsMessengerEXT.Invoke(instance, messenger, pAllocator);

    public void DestroyDebugUtilsMessenger(VkInstance instance, VkDebugUtilsMessengerEXT messenger, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyDebugUtilsMessengerEXT.Invoke(instance, messenger, allocator.Equals(default(VkAllocationCallbacks)) ? null : pAllocator);
        }
    }
}
