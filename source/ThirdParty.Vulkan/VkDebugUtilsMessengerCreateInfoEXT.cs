using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsMessengerCreateInfoEXT.html">VkDebugUtilsMessengerCreateInfoEXT</see>
/// </summary>
/// </summary>
public unsafe struct VkDebugUtilsMessengerCreateInfoEXT
{
    public readonly VkStructureType SType;

    public void*                                PNext;
    public VkDebugUtilsMessengerCreateFlagsEXT  Flags;
    public VkDebugUtilsMessageSeverityFlagsEXT  MessageSeverity;
    public VkDebugUtilsMessageTypeFlagsEXT      MessageType;
    public PFN_vkDebugUtilsMessengerCallbackEXT PfnUserCallback;
    public void*                                PUserData;

    public VkDebugUtilsMessengerCreateInfoEXT() =>
        this.SType = VkStructureType.DebugUtilsMessengerCreateInfoEXT;
}
