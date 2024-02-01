using System.Runtime.InteropServices;
using ThirdParty.Vulkan.EXT;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.Native;
using ThirdParty.Vulkan.Native.EXT;

namespace ThirdParty.Vulkan.Extensions.EXT;

public unsafe class DebugUtilsExtension : IInstanceExtension<DebugUtilsExtension>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateDebugUtilsMessengerEXT(VkInstance instance, VkDebugUtilsMessengerCreateInfoEXT* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDebugUtilsMessengerEXT* pMessenger);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyDebugUtilsMessengerEXT(VkInstance instance, VkDebugUtilsMessengerEXT messenger, VkAllocationCallbacks* pAllocator);

    public static string Name { get; } = "VK_EXT_debug_utils";

    private readonly Instance instance;

    private readonly VkCreateDebugUtilsMessengerEXT  vkCreateDebugUtilsMessengerEXT;
    private readonly VkDestroyDebugUtilsMessengerEXT vkDestroyDebugUtilsMessengerEXT;

    internal DebugUtilsExtension(Instance instance)
    {
        this.instance = instance;

        this.vkCreateDebugUtilsMessengerEXT  = instance.GetProcAddr<VkCreateDebugUtilsMessengerEXT>("vkCreateDebugUtilsMessengerEXT");
        this.vkDestroyDebugUtilsMessengerEXT = instance.GetProcAddr<VkDestroyDebugUtilsMessengerEXT>("vkDestroyDebugUtilsMessengerEXT");
    }

    public static DebugUtilsExtension Create(Instance instance) =>
        new(instance);

    public DebugUtilsMessenger CreateDebugUtilsMessenger(DebugUtilsMessenger.CreateInfo createInfo)
    {
        VkDebugUtilsMessengerEXT messenger;

        this.vkCreateDebugUtilsMessengerEXT.Invoke(this.instance, createInfo, this.instance.Allocator, &messenger);

        return new(messenger, this);
    }

    public void DestroyDebugUtilsMessenger(DebugUtilsMessenger messenger) =>
        this.vkDestroyDebugUtilsMessengerEXT.Invoke(this.instance, messenger, this.instance.Allocator);
}
