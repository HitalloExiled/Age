namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferImageCopy.html">VkBufferImageCopy</see>
/// </summary>
public struct VkBufferImageCopy
{
    public VkDeviceSize             bufferOffset;
    public uint                     bufferRowLength;
    public uint                     bufferImageHeight;
    public VkImageSubresourceLayers imageSubresource;
    public VkOffset3D               imageOffset;
    public VkExtent3D               imageExtent;
}
