namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorBufferInfo.html">VkDescriptorBufferInfo</see>
/// </summary>
public struct VkDescriptorBufferInfo
{
    public VkBuffer     buffer;
    public VkDeviceSize offset;
    public VkDeviceSize range;
}
