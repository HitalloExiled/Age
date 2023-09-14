namespace Age.Vulkan.Native;

public unsafe struct VkExtensionProperties
{
    public fixed char extensionName[(int)Vulkan.VK_MAX_EXTENSION_NAME_SIZE];
    public uint       specVersion;
}
