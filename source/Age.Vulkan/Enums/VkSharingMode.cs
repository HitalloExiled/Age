namespace Age.Vulkan.Enums;

/// <summary>
/// <para>Buffer and image sharing modes</para>
/// <para>Ranges of buffers and image subresources of image objects created using <see cref="VK_SHARING_MODE_EXCLUSIVE"/> must only be accessed by queues in the queue family that has ownership of the resource. Upon creation, such resources are not owned by any queue family; ownership is implicitly acquired upon first use within a queue. Once a resource using <see cref="VK_SHARING_MODE_EXCLUSIVE"/> is owned by some queue family, the application must perform a queue family ownership transfer to make the memory contents of a range or image subresource accessible to a different queue family.</para>
/// <remarks>Images still require a layout transition from <see cref="VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED"/> or <see cref="VkImageLayout.VK_IMAGE_LAYOUT_PREINITIALIZED"/> before being used on the first queue.</remarks>
/// <para>A queue family can take ownership of an image subresource or buffer range of a resource created with <see cref="VK_SHARING_MODE_EXCLUSIVE"/>, without an ownership transfer, in the same way as for a resource that was just created; however, taking ownership in this way has the effect that the contents of the image subresource or buffer range are undefined.</para>
/// <para>Ranges of buffers and image subresources of image objects created using <see cref="VK_SHARING_MODE_CONCURRENT"/> must only be accessed by queues from the queue families specified through the queueFamilyIndexCount and pQueueFamilyIndices members of the corresponding create info structures.</para>
/// </summary>
public enum VkSharingMode
{
    /// <summary>
    /// specifies that access to any range or image subresource of the object will be exclusive to a single queue family at a time.
    /// </summary>
    VK_SHARING_MODE_EXCLUSIVE = 0,

    /// <summary>
    /// specifies that concurrent access to any range or image subresource of the object from multiple queue families is supported.
    /// </summary>
    /// <remarks><see cref="VK_SHARING_MODE_CONCURRENT"/> may result in lower performance access to the buffer or image than VK_SHARING_MODE_EXCLUSIVE.</remarks>
    VK_SHARING_MODE_CONCURRENT = 1,
}
