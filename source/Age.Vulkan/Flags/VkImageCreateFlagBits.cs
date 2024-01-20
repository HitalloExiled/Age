
namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask specifying additional parameters of an image.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkImageCreateFlagBits
{

    /// <summary>
    /// Specifies that the image will be backed using sparse memory binding.
    /// </summary>
    VK_IMAGE_CREATE_SPARSE_BINDING_BIT = 0x00000001,

    /// <summary>
    /// Specifies that the image can be partially backed using sparse memory binding. Images created with this flag must also be created with the <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_SPARSE_BINDING_BIT"/> flag.
    /// </summary>
    VK_IMAGE_CREATE_SPARSE_RESIDENCY_BIT = 0x00000002,

    /// <summary>
    /// Specifies that the image will be backed using sparse memory binding with memory ranges that might also simultaneously be backing another image (or another portion of the same image). Images created with this flag must also be created with the <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_SPARSE_BINDING_BIT"/> flag.
    /// </summary>
    VK_IMAGE_CREATE_SPARSE_ALIASED_BIT = 0x00000004,

    /// <summary>
    /// Specifies that the image can be used to create a <see cref="VkImageView"/> with a different format from the image. For <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#formats-requiring-sampler-ycbcr-conversion">multi-planar</see> formats, <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_MUTABLE_FORMAT_BIT"/> specifies that a <see cref="VkImageView"/> can be created of a plane of the image.
    /// </summary>
    VK_IMAGE_CREATE_MUTABLE_FORMAT_BIT = 0x00000008,

    /// <summary>
    /// Specifies that the image can be used to create a <see cref="VkImageView"/> of type <see cref="VkImageViewTypeC.VK_IMAGE_VIEW_TYPE_CUBE"/> or <see cref="VkImageViewTypeCubeAr.VK_IMAGE_VIEW_TYPE_CUBE_ARRAY"/>.
    /// </summary>
    VK_IMAGE_CREATE_CUBE_COMPATIBLE_BIT = 0x00000010,

    /// <summary>
    /// Specifies that two images created with the same creation parameters and aliased to the same memory can interpret the contents of the memory consistently with each other, subject to the rules described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#resources-memory-aliasing">Memory Aliasing</see> section. This flag further specifies that each plane of a disjoint image can share an in-memory non-linear representation with single-plane images, and that a single-plane image can share an in-memory non-linear representation with a plane of a multi-planar disjoint image, according to the rules in https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#formats-compatible-planes. If the pNext chain includes a <see cref="VkExternalMemoryImageCreateInfo"/> or <see cref="VkExternalMemoryImageCreateInfoNV"/> structure whose handleTypes member is not 0, it is as if <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_ALIAS_BIT"/> is set.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_CREATE_ALIAS_BIT = 0x00000400,

    /// <summary>
    /// Specifies that the image can be used with a non-zero value of the splitInstanceBindRegionCount member of a <see cref="VkBindImageMemoryDeviceGroupInfo"/> structure passed into <see cref="Vk.BindImageMemory2"/>. This flag also has the effect of making the image use the standard sparse image block dimensions.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_CREATE_SPLIT_INSTANCE_BIND_REGIONS_BIT = 0x00000040,

    /// <summary>
    /// Specifies that the image can be used to create a <see cref="VkImageView"/> of type <see cref="VkImageViewType.VK_IMAGE_VIEW_TYPE_2D"/> or <see cref="VkImageViewType_2DAr.VK_IMAGE_VIEW_TYPE_2D_ARRAY"/>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_CREATE_2D_ARRAY_COMPATIBLE_BIT = 0x00000020,

    /// <summary>
    /// Specifies that the image having a compressed format can be used to create a <see cref="VkImageView"/> with an uncompressed format where each texel in the image view corresponds to a compressed texel block of the image.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_CREATE_BLOCK_TEXEL_VIEW_COMPATIBLE_BIT = 0x00000080,

    /// <summary>
    /// Specifies that the image can be created with usage flags that are not supported for the format the image is created with but are supported for at least one format a <see cref="VkImageView"/> created from the image can have.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_CREATE_EXTENDED_USAGE_BIT = 0x00000100,

    /// <summary>
    /// Specifies that the image is a protected image.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_CREATE_PROTECTED_BIT = 0x00000800,

    /// <summary>
    /// Specifies that an image with a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#formats-requiring-sampler-ycbcr-conversion">multi-planar format</see> must have each plane separately bound to memory, rather than having a single memory binding for the whole image; the presence of this bit distinguishes a disjoint image from an image without this bit set.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_IMAGE_CREATE_DISJOINT_BIT = 0x00000200,

    /// <summary>
    /// Specifies that the image is a corner-sampled image.
    /// </summary>
    /// <remarks>Provided by VK_NV_corner_sampled_image</remarks>
    VK_IMAGE_CREATE_CORNER_SAMPLED_BIT_NV = 0x00002000,

    /// <summary>
    /// Specifies that an image with a depth or depth/stencil format can be used with custom sample locations when used as a depth/stencil attachment.
    /// </summary>
    /// <remarks>Provided by VK_EXT_sample_locations</remarks>
    VK_IMAGE_CREATE_SAMPLE_LOCATIONS_COMPATIBLE_DEPTH_BIT_EXT = 0x00001000,

    /// <summary>
    /// Specifies that an image can be in a subsampled format which may be more optimal when written as an attachment by a render pass that has a fragment density map attachment. Accessing a subsampled image has additional considerations:
    /// <list type="bullet">
    /// <item>Image data read as an image sampler will have undefined values if the sampler was not created with flags containing <see cref="VkSamplerCreateFlagBits.VK_SAMPLER_CREATE_SUBSAMPLED_BIT_EXT"/> or was not sampled through the use of a combined image sampler with an immutable sampler in <see cref="VkDescriptorSetLayoutBinding"/>.</item>
    /// <item>Image data read with an input attachment will have undefined values if the contents were not written as an attachment in an earlier subpass of the same render pass.</item>
    /// <item>Image data read as an image sampler in the fragment shader will be additionally be read by the device during <see cref="VkPipelineStageFlagBits.VK_PIPELINE_STAGE_VERTEX_SHADER_BIT"/> if <see cref="VkPhysicalDeviceFragmentDensityMap2PropertiesEXTsubsampledCoarseReconstructionEarlyAccess."/> is true and the sampler was created with flags containing <see cref="VkSamplerCreateFlagBits.VK_SAMPLER_CREATE_SUBSAMPLED_COARSE_RECONSTRUCTION_BIT_EXT"/>.</item>
    /// <item>Image data read with load operations are resampled to the fragment density of the render pass if <see cref="VkPhysicalDeviceFragmentDensityMap2PropertiesEXT.subsampledLoads"/> is true. Otherwise, values of image data are undefined.</item>
    /// <item>Image contents outside of the render area take on undefined values if the image is stored as a render pass attachment.</item>
    /// </list>
    /// </summary>
    /// <remarks>Provided by VK_EXT_fragment_density_map</remarks>
    VK_IMAGE_CREATE_SUBSAMPLED_BIT_EXT = 0x00004000,

    /// <summary>
    /// Specifies that the image can be used with descriptor buffers when capturing and replaying (e.g. for trace capture and replay), see <see cref="VkOpaqueCaptureDescriptorDataCreateInfoEXT"/> for more detail.
    /// </summary>
    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_IMAGE_CREATE_DESCRIPTOR_BUFFER_CAPTURE_REPLAY_BIT_EXT = 0x00010000,

    /// <summary>
    /// Specifies that an image can be used with <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#multisampled-render-to-single-sampled">multisampled rendering as a single-sampled framebuffer attachment</see>.
    /// </summary>
    /// <remarks>Provided by VK_EXT_multisampled_render_to_single_sampled</remarks>
    VK_IMAGE_CREATE_MULTISAMPLED_RENDER_TO_SINGLE_SAMPLED_BIT_EXT = 0x00040000,

    /// <summary>
    /// Specifies that the image can be used to create a <see cref="VkImageView"/> of type <see cref="VkImageViewType.VK_IMAGE_VIEW_TYPE_2D"/>.
    /// </summary>
    /// <remarks>Provided by VK_EXT_image_2d_view_of_3d</remarks>
    VK_IMAGE_CREATE_2D_VIEW_COMPATIBLE_BIT_EXT = 0x00020000,

    /// <summary>
    /// Specifies that an image can be used in a render pass with non-zero <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#renderpass-fragmentdensitymapoffsets">fragment density map offsets</see>. In a render pass with non-zero offsets, fragment density map attachments, input attachments, color attachments, depth/stencil attachment, resolve attachments, and preserve attachments must be created with <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_FRAGMENT_DENSITY_MAP_OFFSET_BIT_QCOM"/>.
    /// </summary>
    /// <remarks>Provided by VK_QCOM_fragment_density_map_offset</remarks>
    VK_IMAGE_CREATE_FRAGMENT_DENSITY_MAP_OFFSET_BIT_QCOM = 0x00008000,

    /// <inheritdoc cref="VK_IMAGE_CREATE_SPLIT_INSTANCE_BIND_REGIONS_BIT" />
    /// <remarks>Provided by VK_KHR_bind_memory2 with VK_KHR_device_group</remarks>
    VK_IMAGE_CREATE_SPLIT_INSTANCE_BIND_REGIONS_BIT_KHR = VK_IMAGE_CREATE_SPLIT_INSTANCE_BIND_REGIONS_BIT,

    /// <inheritdoc cref="VK_IMAGE_CREATE_2D_ARRAY_COMPATIBLE_BIT" />
    /// <remarks>Provided by VK_KHR_maintenance1</remarks>
    VK_IMAGE_CREATE_2D_ARRAY_COMPATIBLE_BIT_KHR = VK_IMAGE_CREATE_2D_ARRAY_COMPATIBLE_BIT,

    /// <inheritdoc cref="VK_IMAGE_CREATE_BLOCK_TEXEL_VIEW_COMPATIBLE_BIT" />
    /// <remarks>Provided by VK_KHR_maintenance2</remarks>
    VK_IMAGE_CREATE_BLOCK_TEXEL_VIEW_COMPATIBLE_BIT_KHR = VK_IMAGE_CREATE_BLOCK_TEXEL_VIEW_COMPATIBLE_BIT,

    /// <inheritdoc cref="VK_IMAGE_CREATE_EXTENDED_USAGE_BIT" />
    /// <remarks>Provided by VK_KHR_maintenance2</remarks>
    VK_IMAGE_CREATE_EXTENDED_USAGE_BIT_KHR = VK_IMAGE_CREATE_EXTENDED_USAGE_BIT,

    /// <inheritdoc cref="VK_IMAGE_CREATE_DISJOINT_BIT" />
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_IMAGE_CREATE_DISJOINT_BIT_KHR = VK_IMAGE_CREATE_DISJOINT_BIT,

    /// <inheritdoc cref="VK_IMAGE_CREATE_ALIAS_BIT" />
    /// <remarks>Provided by VK_KHR_bind_memory2</remarks>
    VK_IMAGE_CREATE_ALIAS_BIT_KHR = VK_IMAGE_CREATE_ALIAS_BIT,
}
