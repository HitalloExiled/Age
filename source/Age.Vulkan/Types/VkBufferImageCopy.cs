namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying a buffer image copy operation.
/// </summary>
public struct VkBufferImageCopy
{
    /// <summary>
    /// Is the offset in bytes from the start of the buffer object where the image data is copied from or to.
    /// </summary>
    public VkDeviceSize bufferOffset;

    /// <summary>
    /// <see cref="bufferRowLength"/> and <see cref="bufferImageHeight"/> specify in texels a subregion of a larger two- or three-dimensional image in buffer memory, and control the addressing calculations. If either of these values is zero, that aspect of the buffer memory is considered to be tightly packed according to the imageExtent.
    /// </summary>
    public uint bufferRowLength;

    /// <inheritdoc cref="bufferRowLength" />
    public uint bufferImageHeight;

    /// <summary>
    /// Is a <see cref="VkImageSubresourceLayers"/> used to specify the specific image subresources of the image used for the source or destination image data.
    /// </summary>
    public VkImageSubresourceLayers imageSubresource;

    /// <summary>
    /// Selects the initial x, y, z offsets in texels of the sub-region of the source or destination image data.
    /// </summary>
    public VkOffset3D imageOffset;

    /// <summary>
    /// Is the size in texels of the image to copy in width, height and depth.
    /// </summary>
    public VkExtent3D imageExtent;
}
