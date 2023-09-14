using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Extensions.EXT.Enums;
using Age.Vulkan.Native.Extensions.EXT.PFN;
using Age.Vulkan.Native.Extensions.EXT.Flags;

namespace Age.Vulkan.Native.Extensions.EXT.Types;

/// <summary>
/// <para>For each <see cref="VkDebugUtilsMessengerEXT"/> that is created the <see cref="messageSeverity"/> and <see cref="messageType"/> determine when that <see cref="pfnUserCallback"/> is called. The process to determine if the user’s <see cref="pfnUserCallback"/> is triggered when an event occurs is as follows:</para>
/// <list type="bullet">
/// <item>
/// <para>The implementation will perform a bitwise AND of the event’s <see cref="VkDebugUtilsMessageSeverityFlagBitsEXT"/> with the messageSeverity provided during creation of the <see cref="VkDebugUtilsMessengerEXT"/> object.</para>
/// <para>If the value is 0, the message is skipped.</para>
/// </item>
/// <item>
/// <para>The implementation will perform bitwise AND of the event’s <see cref="VkDebugUtilsMessageTypeFlagBitsEXT"/> with the messageType provided during the creation of the <see cref="VkDebugUtilsMessengerEXT"/> object.</para>
/// <para>If the value is 0, the message is skipped.</para>
/// </item>
/// <item>The callback will trigger a debug message for the current event</item>
/// </list>
/// <para>The callback will come directly from the component that detected the event, unless some other layer intercepts the calls for its own purposes (filter them in a different way, log to a system error log, etc.).</para>
/// <para>An application can receive multiple callbacks if multiple <see cref="VkDebugUtilsMessengerEXT"/> objects are created. A callback will always be executed in the same thread as the originating Vulkan call.</para>
/// <para>A callback can be called from multiple threads simultaneously (if the application is making Vulkan calls from multiple threads).</para>
/// </summary>
public unsafe struct VkDebugUtilsMessengerCreateInfoEXT
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public VkStructureType sType;

    /// <summary>
    /// NULL or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// 0 and is reserved for future use.
    /// </summary>
    public VkDebugUtilsMessengerCreateFlagsEXT flags;

    /// <summary>
    /// A bitmask of <see cref="VkDebugUtilsMessageSeverityFlagBitsEXT"/> specifying which severity of event(s) will cause this callback to be called.
    /// </summary>
    public VkDebugUtilsMessageSeverityFlagsEXT messageSeverity;

    /// <summary>
    /// A bitmask of <see cref="VkDebugUtilsMessageTypeFlagBitsEXT"/> specifying which type of event(s) will cause this callback to be called.
    /// </summary>
    public VkDebugUtilsMessageTypeFlagsEXT messageType;

    /// <summary>
    /// Is the application callback function to call.
    /// </summary>
    public PFN_vkDebugUtilsMessengerCallbackEXT pfnUserCallback;

    /// <summary>
    /// Is user data to be passed to the callback.
    /// </summary>
    public void* pUserData;
}
