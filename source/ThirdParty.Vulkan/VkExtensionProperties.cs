namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkExtensionProperties.html">VkExtensionProperties</see>
/// </summary>
public unsafe struct VkExtensionProperties
{
    public fixed byte ExtensionName[(int)VkConstants.VK_MAX_EXTENSION_NAME_SIZE];
    public uint       SpecVersion;
}
