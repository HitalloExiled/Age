namespace Age.Vulkan.Native.Extensions.KHR.Enums;

/// <summary>
/// Retrieve the index of the next available presentable image.
/// </summary>
public enum VkSwapchainCreateFlagBitsKHR
{
    /// <summary>
    /// Specifies that images created from the swapchain (i.e. with the swapchain member of <see cref="VkImageSwapchainCreateInfoKHR"/> set to this swapchain’s handle) must use <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_SPLIT_INSTANCE_BIND_REGIONS_BIT"/>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1 with VK_KHR_swapchain, VK_KHR_device_group with VK_KHR_swapchain</remarks>
    VK_SWAPCHAIN_CREATE_SPLIT_INSTANCE_BIND_REGIONS_BIT_KHR = 0x00000001,

    /// <summary>
    /// Specifies that images created from the swapchain are protected images.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1 with VK_KHR_swapchain</remarks>
    VK_SWAPCHAIN_CREATE_PROTECTED_BIT_KHR = 0x00000002,

    /// <summary>
    /// Specifies that the images of the swapchain can be used to create a <see cref="VkImageView"/> with a different format than what the swapchain was created with. The list of allowed image view formats is specified by adding a <see cref="VkImageFormatListCreateInfo"/> structure to the pNext chain of <see cref="VkSwapchainCreateInfoKHR"/>. In addition, this flag also specifies that the swapchain can be created with usage flags that are not supported for the format the swapchain is created with but are supported for at least one of the allowed image view formats.
    /// </summary>
    /// <remarks>Provided by VK_KHR_swapchain_mutable_format</remarks>
    VK_SWAPCHAIN_CREATE_MUTABLE_FORMAT_BIT_KHR = 0x00000004,

    /// <summary>
    /// Specifies that the implementation may defer allocation of memory associated with each swapchain image until its index is to be returned from <see cref="VkKhrSwapchain.AcquireNextImageKHR"/> or <see cref="VkKhrSwapchain.AcquireNextImage2KHR"/> for the first time.
    /// </summary>
    /// <remarks>Provided by VK_EXT_swapchain_maintenance1</remarks>
    VK_SWAPCHAIN_CREATE_DEFERRED_MEMORY_ALLOCATION_BIT_EXT = 0x00000008,

}
