using System.Runtime.InteropServices;
using Age.Core.Interop;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Interfaces;

namespace ThirdParty.Vulkan.Extensions;

public unsafe class VkDebugUtilsExtensionEXT : IInstanceExtension<VkDebugUtilsExtensionEXT>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateDebugUtilsMessengerEXT(VkHandle<VkInstance> instance, VkDebugUtilsMessengerCreateInfoEXT* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkDebugUtilsMessengerEXT>* pMessenger);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyDebugUtilsMessengerEXT(VkHandle<VkInstance> instance, VkHandle<VkDebugUtilsMessengerEXT> messenger, VkAllocationCallbacks* pAllocator);

    public static string Name { get; } = "VK_EXT_debug_utils";

    private readonly VkInstance instance;

    private readonly VkCreateDebugUtilsMessengerEXT  vkCreateDebugUtilsMessengerEXT;
    private readonly VkDestroyDebugUtilsMessengerEXT vkDestroyDebugUtilsMessengerEXT;

    internal VkDebugUtilsExtensionEXT(VkInstance instance)
    {
        this.instance = instance;

        this.vkCreateDebugUtilsMessengerEXT  = instance.GetProcAddr<VkCreateDebugUtilsMessengerEXT>("vkCreateDebugUtilsMessengerEXT");
        this.vkDestroyDebugUtilsMessengerEXT = instance.GetProcAddr<VkDestroyDebugUtilsMessengerEXT>("vkDestroyDebugUtilsMessengerEXT");
    }

    public static VkDebugUtilsExtensionEXT Create(VkInstance instance) =>
        new(instance);

    public VkDebugUtilsMessengerEXT CreateDebugUtilsMessenger(in VkDebugUtilsMessengerCreateInfoEXT createInfo)
    {
        VkHandle<VkDebugUtilsMessengerEXT> messenger;

        fixed (VkDebugUtilsMessengerCreateInfoEXT* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*              pAllocator  = &this.instance.Allocator)
        {
            this.vkCreateDebugUtilsMessengerEXT.Invoke(this.instance.Handle, pCreateInfo, PointerHelper.NullIfDefault(pAllocator), &messenger);
        }

        return new(messenger, this);
    }

    public void DestroyDebugUtilsMessenger(VkDebugUtilsMessengerEXT messenger)
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.instance.Allocator)
        {
            this.vkDestroyDebugUtilsMessengerEXT.Invoke(this.instance.Handle, messenger.Handle, PointerHelper.NullIfDefault(pAllocator));
        }
    }
}
