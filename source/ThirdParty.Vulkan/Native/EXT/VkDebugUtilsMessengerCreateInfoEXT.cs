namespace ThirdParty.Vulkan.Native.EXT;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsMessengerCreateInfoEXT.html">VkDebugUtilsMessengerCreateInfoEXT</see>
/// </summary>
/// </summary>
public unsafe struct VkDebugUtilsMessengerCreateInfoEXT
{
    public readonly VkStructureType sType;

    public void*                                pNext;
    public VkDebugUtilsMessengerCreateFlagsEXT  flags;
    public VkDebugUtilsMessageSeverityFlagsEXT  messageSeverity;
    public VkDebugUtilsMessageTypeFlagsEXT      messageType;
    public PFN_vkDebugUtilsMessengerCallbackEXT pfnUserCallback;
    public void*                                pUserData;

    public VkDebugUtilsMessengerCreateInfoEXT() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT;
}
