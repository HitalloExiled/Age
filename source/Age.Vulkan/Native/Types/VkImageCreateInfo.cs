using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying the parameters of a newly created image object.</para>
/// <para>Images created with tiling equal to <see cref="VkImageTiling.VK_IMAGE_TILING_LINEAR"/> have further restrictions on their limits and capabilities compared to images created with tiling equal to <see cref="VkImageTiling.VK_IMAGE_TILING_OPTIMAL"/>. Creation of images with tiling <see cref="VkImageTiling.VK_IMAGE_TILING_LINEAR"/> may not be supported unless other parameters meet all of the constraints:</para>
/// <list type="bullet">
/// <item>imageType is <see cref="VkImageType.VK_IMAGE_TYPE_2D"/></item>
/// <item>format is not a depth/stencil format</item>
/// <item>mipLevels is 1</item>
/// <item>arrayLayers is 1</item>
/// <item>samples is <see cref="VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT"/></item>
/// <item>usage only includes <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_SRC_BIT"/> and/or <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_DST_BIT"/></item>
/// </list>
/// <para>Images created with one of the formats that require a sampler Y′CBCR conversion, have further restrictions on their limits and capabilities compared to images created with other formats. Creation of images with a format requiring Y′CBCR conversion may not be supported unless other parameters meet all of the constraints:</para>
/// <list type="bullet">
/// <item>imageType is <see cref="VkImageType.VK_IMAGE_TYPE_2D"/></item>
/// <item>mipLevels is 1</item>
/// <item>arrayLayers is 1, unless the ycbcrImageArrays feature is enabled, or otherwise indicated by <see cref="VkImageFormatProperties.maxArrayLayers"/>, as returned by <see cref="vkGetPhysicalDeviceImageFormatProperties"/></item>
/// <item>samples is <see cref="VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT"/></item>
/// </list>
/// <para>Implementations may support additional limits and capabilities beyond those listed above.</para>
/// <para>To determine the set of valid usage bits for a given format, call <see cref="Vk.GetPhysicalDeviceFormatProperties"/>.</para>
/// <para>If the size of the resultant image would exceed maxResourceSize, then <see cref="Vk.CreateImage"/> must fail and return <see cref="VkResult.VK_ERROR_OUT_OF_DEVICE_MEMORY"/>. This failure may occur even when all image creation parameters satisfy their valid usage requirements.</para>
/// <para>If the implementation reports VK_TRUE in <see cref="VkPhysicalDeviceHostImageCopyPropertiesEXT.identicalMemoryTypeRequirements"/>, usage of <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_HOST_TRANSFER_BIT_EXT"/> must not affect the memory type requirements of the image as described in Sparse Resource Memory Requirements and Resource Memory Association.</para>
/// <remarks>Note: For images created without <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_EXTENDED_USAGE_BIT"/> a usage bit is valid if it is supported for the format the image is created with.</remarks>
/// <para>For images created with <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_EXTENDED_USAGE_BIT"/> a usage bit is valid if it is supported for at least one of the formats a <see cref="VkImageView"/> created from the image can have (see Image Views for more detail).</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkImageCreateInfo
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask of <see cref="VkImageCreateFlagBits"/> describing additional parameters of the image.
    /// </summary>
    public VkImageCreateFlags flags;

    /// <summary>
    /// A <see cref="VkImageType"/> value specifying the basic dimensionality of the image. Layers in array textures do not count as a dimension for the purposes of the image type.
    /// </summary>
    public VkImageType imageType;

    /// <summary>
    /// A <see cref="VkFormat"/> describing the format and type of the texel blocks that will be contained in the image.
    /// </summary>
    public VkFormat format;

    /// <summary>
    /// A <see cref="VkExtent3D"/> describing the number of data elements in each dimension of the base level.
    /// </summary>
    public VkExtent3D extent;

    /// <summary>
    /// Describes the number of levels of detail available for minified sampling of the image.
    /// </summary>
    public uint mipLevels;

    /// <summary>
    /// Is the number of layers in the image.
    /// </summary>
    public uint arrayLayers;

    /// <summary>
    /// A <see cref="VkSampleCountFlagBits"/> value specifying the number of samples per texel.
    /// </summary>
    public VkSampleCountFlagBits samples;

    /// <summary>
    /// A <see cref="VkImageTiling"/> value specifying the tiling arrangement of the texel blocks in memory.
    /// </summary>
    public VkImageTiling tiling;

    /// <summary>
    /// A bitmask of <see cref="VkImageUsageFlagBits"/> describing the intended usage of the image.
    /// </summary>
    public VkImageUsageFlags usage;

    /// <summary>
    /// A <see cref="VkSharingMode"/> value specifying the sharing mode of the image when it will be accessed by multiple queue families.
    /// </summary>
    public VkSharingMode sharingMode;

    /// <summary>
    /// Is the number of entries in the pQueueFamilyIndices array.
    /// </summary>
    public uint queueFamilyIndexCount;

    /// <summary>
    /// A pointer to an array of queue families that will access this image. It is ignored if sharingMode is not <see cref="VkSharingMode.VK_SHARING_MODE_CONCURRENT"/>.
    /// </summary>
    public uint* pQueueFamilyIndices;

    /// <summary>
    /// A <see cref="VkImageLayout"/> value specifying the initial <see cref="VkImageLayout"/> of all image subresources of the image. See Image Layouts.
    /// </summary>
    public VkImageLayout initialLayout;

    public VkImageCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO;
}
