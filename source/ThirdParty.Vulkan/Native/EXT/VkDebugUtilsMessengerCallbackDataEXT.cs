namespace ThirdParty.Vulkan.Native.EXT;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsMessengerCallbackDataEXT.html">VkDebugUtilsMessengerCallbackDataEXT</see>
/// </summary>
public unsafe struct VkDebugUtilsMessengerCallbackDataEXT
{
    public readonly VkStructureType sType;

    public void*                                     pNext;
    public VkDebugUtilsMessengerCallbackDataFlagsEXT flags;
    public byte*                                     pMessageIdName;
    public int                                       messageIdNumber;
    public byte*                                     pMessage;
    public uint                                      queueLabelCount;
    public VkDebugUtilsLabelEXT*                     pQueueLabels;
    public uint                                      cmdBufLabelCount;
    public VkDebugUtilsLabelEXT*                     pCmdBufLabels;
    public uint                                      objectCount;
    public VkDebugUtilsObjectNameInfoEXT*            pObjects;

    public VkDebugUtilsMessengerCallbackDataEXT() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CALLBACK_DATA_EXT;
}
