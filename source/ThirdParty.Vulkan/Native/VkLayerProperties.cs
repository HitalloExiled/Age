namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkLayerProperties.html">VkLayerProperties</see>
/// </summary>
public unsafe struct VkLayerProperties
{
    public fixed byte layerName[(int)Constants.VK_MAX_EXTENSION_NAME_SIZE];
    public uint       specVersion;
    public uint       implementationVersion;
    public fixed byte description[(int)Constants.VK_MAX_DESCRIPTION_SIZE];
}
