namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferImageCopy.html">VkBufferImageCopy</see>
/// </summary>
public struct VkBufferImageCopy
{
    public VkDeviceSize             BufferOffset;
    public uint                     BufferRowLength;
    public uint                     BufferImageHeight;
    public VkImageSubresourceLayers ImageSubresource;
    public VkOffset3D               ImageOffset;
    public VkExtent3D               ImageExtent;
}
