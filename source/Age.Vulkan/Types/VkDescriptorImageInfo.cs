using Age.Vulkan.Enums;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying descriptor image information.
/// Members of <see cref="VkDescriptorImageInfo"/> that are not used in an update (as described bellow) are ignored.
/// </summary>
public struct VkDescriptorImageInfo
{
    /// <summary>
    /// A sampler handle, and is used in descriptor updates for types <see cref="VK_DESCRIPTOR_TYPE_SAMPLER"/> and <see cref="VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER"/> if the binding being updated does not use immutable samplers.
    /// </summary>
    public VkSampler sampler;

    /// <summary>
    /// default or an image view handle, and is used in descriptor updates for types <see cref="VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE"/>, <see cref="VK_DESCRIPTOR_TYPE_STORAGE_IMAGE"/>, <see cref="VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER"/>, and <see cref="VK_DESCRIPTOR_TYPE_INPUT_ATTACHMENT"/>.
    /// </summary>
    public VkImageView imageView;

    /// <summary>
    /// The layout that the image subresources accessible from imageView will be in at the time this descriptor is accessed. imageLayout is used in descriptor updates for types <see cref="VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE"/>, <see cref="VK_DESCRIPTOR_TYPE_STORAGE_IMAGE"/>, <see cref="VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER"/>, and <see cref="VK_DESCRIPTOR_TYPE_INPUT_ATTACHMENT"/>.
    /// </summary>
    public VkImageLayout imageLayout;
}
