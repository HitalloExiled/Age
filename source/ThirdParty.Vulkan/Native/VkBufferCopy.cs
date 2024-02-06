namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferCopy.html">VkBufferCopy</see>
/// </summary>
public struct VkBufferCopy
{
    public VkDeviceSize SrcOffset;
    public VkDeviceSize DstOffset;
    public VkDeviceSize Size;
}
