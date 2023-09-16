using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags.EXT;

namespace Age.Vulkan.Native.Types.EXT;

/// <summary>
/// Since adding queue and command buffer labels behaves like pushing and popping onto a stack, the order of both <see cref="pQueueLabels"/> and <see cref="pCmdBufLabels"/> is based on the order the labels were defined. The result is that the first label in either <see cref="pQueueLabels"/> or <see cref="pCmdBufLabels"/> will be the first defined (and therefore the oldest) while the last label in each list will be the most recent.
/// </summary>
public unsafe struct VkDebugUtilsMessengerCallbackDataEXT
{
    /// <summary>
    /// value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// is 0 and is reserved for future use.
    /// </summary>
    public VkDebugUtilsMessengerCallbackDataFlagsEXT flags;

    /// <summary>
    /// a null-terminated string that identifies the particular message ID that is associated with the provided message. If the message corresponds to a validation layer message, then this string may contain the portion of the Vulkan specification that is believed to have been violated.
    /// </summary>
    public byte* pMessageIdName;

    /// <summary>
    /// the ID number of the triggering message. If the message corresponds to a validation layer message, then this number is related to the internal number associated with the message being triggered.
    /// </summary>
    public int messageIdNumber;

    /// <summary>
    /// a null-terminated string detailing the trigger conditions.
    /// </summary>
    public byte* pMessage;

    /// <summary>
    /// is a count of items contained in the <see cref="pQueueLabels"/> array.
    /// </summary>
    public uint queueLabelCount;

    /// <summary>
    /// Null or a pointer to an array of <see cref="VkDebugUtilsLabelEXT"/> active in the current VkQueue at the time the callback was triggered. Refer to Queue Labels for more information.
    /// </summary>
    public VkDebugUtilsLabelEXT* pQueueLabels;

    /// <summary>
    /// a count of items contained in the pCmdBufLabels array.
    /// </summary>
    public uint cmdBufLabelCount;

    /// <summary>
    /// Null or a pointer to an array of <see cref="VkDebugUtilsLabelEXT"/> active in the current <see cref="VkCommandBuffer"/> at the time the callback was triggered. Refer to Command Buffer Labels for more information.
    /// </summary>
    public VkDebugUtilsLabelEXT* pCmdBufLabels;

    /// <summary>
    /// a count of items contained in the <see cref="pObjects"/> array.
    /// </summary>
    public uint objectCount;

    /// <summary>
    /// a pointer to an array of <see cref="VkDebugUtilsObjectNameInfoEXT"/> objects related to the detected issue. The array is roughly in order or importance, but the 0th element is always guaranteed to be the most important object for this message.
    /// </summary>
    public VkDebugUtilsObjectNameInfoEXT* pObjects;

    public VkDebugUtilsMessengerCallbackDataEXT() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CALLBACK_DATA_EXT;
}
