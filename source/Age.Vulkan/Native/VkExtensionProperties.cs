namespace Age.Vulkan.Native;

/// <summary>
/// Structure specifying an extension properties
/// </summary>
public unsafe struct VkExtensionProperties
{
    /// <summary>
    /// An array of <see cref="VK.VK_MAX_EXTENSION_NAME_SIZE"/> char containing a null-terminated UTF-8 string which is the name of the extension.
    /// </summary>
    public fixed byte extensionName[(int)VK.VK_MAX_EXTENSION_NAME_SIZE];

    /// <summary>
    /// The version of this extension. It is an integer, incremented with backward compatible changes.
    /// </summary>
    public uint specVersion;
}
