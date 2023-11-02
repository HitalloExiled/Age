using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying descriptor pool size.</para>
/// <remarks>Note: When creating a descriptor pool that will contain descriptors for combined image samplers of multi-planar formats, an application needs to account for non-trivial descriptor consumption when choosing the descriptorCount value, as indicated by <see cref="VkSamplerYcbcrConversionImageFormatProperties.combinedImageSamplerDescriptorCount"/>.</remarks>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkDescriptorPoolSize
{
    /// <summary>
    /// The type of descriptor.
    /// </summary>
    public VkDescriptorType type;

    /// <summary>
    /// The number of descriptors of that type to allocate. If type is <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK"/> then descriptorCount is the number of bytes to allocate for descriptors of this type.
    /// </summary>
    public uint descriptorCount;
}
