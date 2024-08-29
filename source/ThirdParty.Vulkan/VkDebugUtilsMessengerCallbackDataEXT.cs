using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsMessengerCallbackDataEXT.html">VkDebugUtilsMessengerCallbackDataEXT</see>
/// </summary>
public unsafe struct VkDebugUtilsMessengerCallbackDataEXT
{
    public readonly VkStructureType SType;

    public void*                                     PNext;
    public VkDebugUtilsMessengerCallbackDataFlagsEXT Flags;
    public byte*                                     PMessageIdName;
    public int                                       MessageIdNumber;
    public byte*                                     PMessage;
    public uint                                      QueueLabelCount;
    public VkDebugUtilsLabelEXT*                     PQueueLabels;
    public uint                                      CmdBufLabelCount;
    public VkDebugUtilsLabelEXT*                     PCmdBufLabels;
    public uint                                      ObjectCount;
    public VkDebugUtilsObjectNameInfoEXT*            PObjects;

    public VkDebugUtilsMessengerCallbackDataEXT() =>
        this.SType = VkStructureType.DebugUtilsMessengerCallbackDataEXT;
}
