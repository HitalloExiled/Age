namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying a buffer copy operation.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkBufferCopy
{
    /// <summary>
    /// The starting offset in bytes from the start of srcBuffer.
    /// </summary>
    public VkDeviceSize srcOffset;

    //// <summary>
    /// The starting offset in bytes from the start of dstBuffer.
    /// </summary>
    public VkDeviceSize dstOffset;

    //// <summary>
    /// The number of bytes to copy.
    /// </summary>
    public VkDeviceSize size;
}
