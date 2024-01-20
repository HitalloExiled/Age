

    /// The following bits may be set in bufferFeatures, specifying that the features are supported by buffers or buffer views created with the queried <see cref="Vk.GetPhysicalDeviceFormatProperties"/>::format:


namespace Age.Vulkan.Flags;

/// <summary>
/// <para>Bitmask specifying features supported by a buffer</para>
/// <para>The following bits may be set in bufferFeatures, specifying that the features are supported by buffers or buffer views created with the queried <see cref="Vk.GetPhysicalDeviceFormatProperties"/> format:</para>
/// <list type="bullet">
/// <item>VK_FORMAT_FEATURE_UNIFORM_TEXEL_BUFFER_BIT</item>
/// <item>VK_FORMAT_FEATURE_STORAGE_TEXEL_BUFFER_BIT</item>
/// <item>VK_FORMAT_FEATURE_STORAGE_TEXEL_BUFFER_ATOMIC_BIT</item>
/// <item>VK_FORMAT_FEATURE_VERTEX_BUFFER_BIT</item>
/// <item>VK_FORMAT_FEATURE_ACCELERATION_STRUCTURE_VERTEX_BUFFER_BIT_KHR</item>
/// </list>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkFormatFeatureFlagBits
{

    /// <summary>
    /// Specifies that an image view can be sampled from.
    /// </summary>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_BIT = 0x00000001,

    /// <summary>
    /// Specifies that an image view can be used as a storage image.
    /// </summary>
    VK_FORMAT_FEATURE_STORAGE_IMAGE_BIT = 0x00000002,

    /// <summary>
    /// Specifies that an image view can be used as storage image that supports atomic operations.
    /// </summary>
    VK_FORMAT_FEATURE_STORAGE_IMAGE_ATOMIC_BIT = 0x00000004,

    /// <summary>
    /// Specifies that the format can be used to create a buffer view that can be bound to a <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_TEXEL_BUFFER"/> descriptor.
    /// </summary>
    VK_FORMAT_FEATURE_UNIFORM_TEXEL_BUFFER_BIT = 0x00000008,

    /// <summary>
    /// Specifies that the format can be used to create a buffer view that can be bound to a <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_TEXEL_BUFFER"/> descriptor.
    /// </summary>
    VK_FORMAT_FEATURE_STORAGE_TEXEL_BUFFER_BIT = 0x00000010,

    /// <summary>
    /// Specifies that atomic operations are supported on <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_TEXEL_BUFFER"/> with this format.
    /// </summary>
    VK_FORMAT_FEATURE_STORAGE_TEXEL_BUFFER_ATOMIC_BIT = 0x00000020,

    /// <summary>
    /// Specifies that the format can be used as a vertex attribute format (VkVertexInputAttributeDescription::format).
    /// </summary>

    VK_FORMAT_FEATURE_VERTEX_BUFFER_BIT = 0x00000040,

    /// <summary>
    /// Specifies that an image view can be used as a framebuffer color attachment and as an input attachment.
    /// </summary>
    VK_FORMAT_FEATURE_COLOR_ATTACHMENT_BIT = 0x00000080,

    /// <summary>
    /// Specifies that an image view can be used as a framebuffer color attachment that supports blending.
    /// </summary>
    VK_FORMAT_FEATURE_COLOR_ATTACHMENT_BLEND_BIT = 0x00000100,

    /// <summary>
    /// Specifies that an image view can be used as a framebuffer depth/stencil attachment and as an input attachment.
    /// </summary>
    VK_FORMAT_FEATURE_DEPTH_STENCIL_ATTACHMENT_BIT = 0x00000200,

    /// <summary>
    /// Specifies that an image can be used as srcImage for the <see cref="Vk.CmdBlitImage2"/> and <see cref="Vk.CmdBlitImage"/> commands.
    /// </summary>
    VK_FORMAT_FEATURE_BLIT_SRC_BIT = 0x00000400,

    /// <summary>
    /// Specifies that an image can be used as dstImage for the <see cref="Vk.CmdBlitImage2"/> and <see cref="Vk.CmdBlitImage"/> commands.<see cref=""/>
    /// </summary>
    VK_FORMAT_FEATURE_BLIT_DST_BIT = 0x00000800,

    /// <summary>
    /// <para>VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_LINEAR_BIT specifies that if <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_SAMPLED_IMAGE_BIT"/> is also set, an image view can be used with a sampler that has either of magFilter or minFilter set to <see cref="VkFilter.VK_FILTER_LINEAR"/>, or mipmapMode set to <see cref="VkSamplerMipmapMode.VK_SAMPLER_MIPMAP_MODE_LINEAR"/>. If <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_BLIT_SRC_BIT"/> is also set, an image can be used as the srcImage to <see cref="Vk.CmdBlitImage2"/> and <see cref="Vk.CmdBlitImage"/> with a filter of <see cref="VkFilter.VK_FILTER_LINEAR"/>. This bit must only be exposed for formats that also support the <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_SAMPLED_IMAGE_BIT"/> or <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_BLIT_SRC_BIT"/>.</para>
    /// <para>If the format being queried is a depth/stencil format, this bit only specifies that the depth aspect (not the stencil aspect) of an image of this format supports linear filtering, and that linear filtering of the depth aspect is supported whether depth compare is enabled in the sampler or not. Where depth comparison is supported it may be linear filtered whether this bit is present or not, but where this bit is not present the filtered value may be computed in an implementation-dependent manner which differs from the normal rules of linear filtering. The resulting value must be in the range [0,1] and should be proportional to, or a weighted average of, the number of comparison passes or failures.</para>
    /// </summary>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_LINEAR_BIT = 0x00001000,

    /// <summary>
    /// Specifies that an image can be used as a source image for copy commands. If the application apiVersion is Vulkan 1.0 and <see cref="VkKhrMaintenance1"/> is not supported, <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_TRANSFER_SRC_BIT"/> is implied to be set when the format feature flag is not 0.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_FORMAT_FEATURE_TRANSFER_SRC_BIT = 0x00004000,

    /// <summary>
    /// Specifies that an image can be used as a destination image for copy commands and clear commands. If the application apiVersion is Vulkan 1.0 and <see cref="VkKhrMaintenance1"/> is not supported, <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_TRANSFER_DST_BIT"/> is implied to be set when the format feature flag is not 0.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_FORMAT_FEATURE_TRANSFER_DST_BIT = 0x00008000,

    /// <summary>
    /// Specifies that an application can define a sampler Y′CBCR conversion using this format as a source, and that an image of this format can be used with a <see cref="VkSamplerYcbcrConversionCreateInfo"/> xChromaOffset and/or yChromaOffset of <see cref="VkChromaLocation.VK_CHROMA_LOCATION_MIDPOINT"/>. Otherwise both xChromaOffset and yChromaOffset must be <see cref="VkChromaLocation.VK_CHROMA_LOCATION_COSITED_EVEN"/>. If a format does not incorporate chroma downsampling (it is not a “422” or “420” format) but the implementation supports sampler Y′CBCR conversion for this format, the implementation must set <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_MIDPOINT_CHROMA_SAMPLES_BIT"/>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_FORMAT_FEATURE_MIDPOINT_CHROMA_SAMPLES_BIT = 0x00020000,

    /// <summary>
    /// Specifies that an application can define a sampler Y′CBCR conversion using this format as a source with chromaFilter set to <see cref="VkFilter.VK_FILTER_LINEAR"/>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_LINEAR_FILTER_BIT = 0x00040000,

    /// <summary>
    /// Specifies that the format can have different chroma, min, and mag filters.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_SEPARATE_RECONSTRUCTION_FILTER_BIT = 0x00080000,

    /// <summary>
    /// Specifies that reconstruction is explicit, as described in https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#textures-chroma-reconstruction. If this bit is not present, reconstruction is implicit by default.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_CHROMA_RECONSTRUCTION_EXPLICIT_BIT = 0x00100000,

    /// <summary>
    /// Specifies that reconstruction can be forcibly made explicit by setting <see cref="VkSamplerYcbcrConversionCreateInfo.forceExplicitReconstruction"/> to true. If the format being queried supports <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_CHROMA_RECONSTRUCTION_EXPLICIT_BIT"/> it must also support <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_CHROMA_RECONSTRUCTION_EXPLICIT_FORCEABLE_BIT"/>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_CHROMA_RECONSTRUCTION_EXPLICIT_FORCEABLE_BIT = 0x00200000,

    /// <summary>
    /// Specifies that a multi-planar image can have the <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_DISJOINT_BIT"/> set during image creation. An implementation must not set <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_DISJOINT_BIT"/> for single-plane formats.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_FORMAT_FEATURE_DISJOINT_BIT = 0x00400000,

    /// <summary>
    /// Specifies that an application can define a sampler Y′CBCR conversion using this format as a source, and that an image of this format can be used with a <see cref="VkSamplerYcbcrConversionCreateInfo"/> xChromaOffset and/or yChromaOffset of <see cref="VkChromaLocation.VK_CHROMA_LOCATION_COSITED_EVEN"/>. Otherwise both xChromaOffset and yChromaOffset must be <see cref="VkChromaLocation.VK_CHROMA_LOCATION_MIDPOINT"/>. If neither <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_COSITED_CHROMA_SAMPLES_BIT"/> nor <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_MIDPOINT_CHROMA_SAMPLES_BIT"/> is set, the application must not define a sampler Y′CBCR conversion using this format as a source.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_FORMAT_FEATURE_COSITED_CHROMA_SAMPLES_BIT = 0x00800000,

    /// <summary>
    /// Specifies <see cref="VkImage"/> can be used as a sampled image with a min or max <see cref="VkSamplerReductionMode"/>. This bit must only be exposed for formats that also support the <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_SAMPLED_IMAGE_BIT"/>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_MINMAX_BIT = 0x00010000,

    /// <summary>
    /// Specifies that an image view with this format can be used as a decode output picture in video decode operations.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_FORMAT_FEATURE_VIDEO_DECODE_OUTPUT_BIT_KHR = 0x02000000,

    /// <summary>
    /// Specifies that an image view with this format can be used as an output reconstructed picture or an input reference picture in video decode operations.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_FORMAT_FEATURE_VIDEO_DECODE_DPB_BIT_KHR = 0x04000000,

    /// <summary>
    /// <para>Specifies that the format can be used as the vertex format when creating an acceleration structure (<see cref="VkAccelerationStructureGeometryTrianglesDataKHR.vertexFormat"/>). This format can also be used as the vertex format in host memory when doing host acceleration structure builds.</para>
    /// <remarks>Note: VK_FORMAT_FEATURE_STORAGE_IMAGE_ATOMIC_BIT and <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_STORAGE_TEXEL_BUFFER_ATOMIC_BIT"/> are only intended to be advertised for single-component formats, since SPIR-V atomic operations require a scalar type.</remarks>
    /// </summary>
    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_FORMAT_FEATURE_ACCELERATION_STRUCTURE_VERTEX_BUFFER_BIT_KHR = 0x20000000,

    /// <summary>
    /// Specifies that <see cref="VkImage"/> can be used with a sampler that has either of magFilter or minFilter set to <see cref="VkFilter.VK_FILTER_CUBIC_EXT"/>, or be the source image for a blit with filter set to <see cref="VkFilter.VK_FILTER_CUBIC_EXT"/>. This bit must only be exposed for formats that also support the <see cref="VkFormatFeatureFlagBits.VK_FORMAT_FEATURE_SAMPLED_IMAGE_BIT"/>. If the format being queried is a depth/stencil format, this only specifies that the depth aspect is cubic filterable.
    /// </summary>
    /// <remarks>Provided by VK_EXT_filter_cubic</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_CUBIC_BIT_EXT = 0x00002000,

    /// <summary>
    /// Specifies that an image view can be used as a fragment density map attachment.
    /// </summary>
    /// <remarks>Provided by VK_EXT_fragment_density_map</remarks>
    VK_FORMAT_FEATURE_FRAGMENT_DENSITY_MAP_BIT_EXT = 0x01000000,

    /// <summary>
    /// Specifies that an image view can be used as a fragment shading rate attachment. An implementation must not set this feature for formats with a numeric format other than UINT, or set it as a buffer feature.
    /// </summary>
    /// <remarks>Provided by VK_KHR_fragment_shading_rate</remarks>
    VK_FORMAT_FEATURE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR = 0x40000000,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Specifies that an image view with this format can be used as an encode input picture in video encode operations.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_FORMAT_FEATURE_VIDEO_ENCODE_INPUT_BIT_KHR = 0x08000000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// <para>Specifies that an image view with this format can be used as an output reconstructed picture or an input reference picture in video encode operations.</para>
    /// <remarks>Note: Specific video profiles may have additional restrictions on the format and other image creation parameters corresponding to image views used by video coding operations that can be enumerated using the <see cref="VkKhrVideoQueue.GetPhysicalDeviceVideoFormatProperties"/> command.</remarks>
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_FORMAT_FEATURE_VIDEO_ENCODE_DPB_BIT_KHR = 0x10000000,
#endif

    /// <inheritdoc cref="VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_CUBIC_BIT_EXT" />
    /// <remarks>Provided by VK_IMG_filter_cubic</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_CUBIC_BIT_IMG = VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_CUBIC_BIT_EXT,

    /// <inheritdoc cref="VK_FORMAT_FEATURE_TRANSFER_SRC_BIT" />
    /// <remarks>Provided by VK_KHR_maintenance1</remarks>
    VK_FORMAT_FEATURE_TRANSFER_SRC_BIT_KHR = VK_FORMAT_FEATURE_TRANSFER_SRC_BIT,

    /// <inheritdoc cref="VK_FORMAT_FEATURE_TRANSFER_DST_BIT" />
    /// <remarks>Provided by VK_KHR_maintenance1</remarks>
    VK_FORMAT_FEATURE_TRANSFER_DST_BIT_KHR = VK_FORMAT_FEATURE_TRANSFER_DST_BIT,

    /// <inheritdoc cref="VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_MINMAX_BIT" />
    /// <remarks>Provided by VK_EXT_sampler_filter_minmax</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_MINMAX_BIT_EXT = VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_MINMAX_BIT,

    /// <inheritdoc cref="VK_FORMAT_FEATURE_MIDPOINT_CHROMA_SAMPLES_BIT" />
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_FORMAT_FEATURE_MIDPOINT_CHROMA_SAMPLES_BIT_KHR = VK_FORMAT_FEATURE_MIDPOINT_CHROMA_SAMPLES_BIT,

    /// <inheritdoc cref="VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_LINEAR_FILTER_BIT" />
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_LINEAR_FILTER_BIT_KHR = VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_LINEAR_FILTER_BIT,

    /// <inheritdoc cref="VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_SEPARATE_RECONSTRUCTION_FILTER_BIT" />
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_SEPARATE_RECONSTRUCTION_FILTER_BIT_KHR = VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_SEPARATE_RECONSTRUCTION_FILTER_BIT,

    /// <inheritdoc cref="VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_CHROMA_RECONSTRUCTION_EXPLICIT_BIT" />
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_CHROMA_RECONSTRUCTION_EXPLICIT_BIT_KHR = VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_CHROMA_RECONSTRUCTION_EXPLICIT_BIT,

    /// <inheritdoc cref="VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_CHROMA_RECONSTRUCTION_EXPLICIT_FORCEABLE_BIT" />
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_CHROMA_RECONSTRUCTION_EXPLICIT_FORCEABLE_BIT_KHR = VK_FORMAT_FEATURE_SAMPLED_IMAGE_YCBCR_CONVERSION_CHROMA_RECONSTRUCTION_EXPLICIT_FORCEABLE_BIT,

    /// <inheritdoc cref="VK_FORMAT_FEATURE_DISJOINT_BIT" />
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_FORMAT_FEATURE_DISJOINT_BIT_KHR = VK_FORMAT_FEATURE_DISJOINT_BIT,

    /// <inheritdoc cref="VK_FORMAT_FEATURE_COSITED_CHROMA_SAMPLES_BIT" />
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_FORMAT_FEATURE_COSITED_CHROMA_SAMPLES_BIT_KHR = VK_FORMAT_FEATURE_COSITED_CHROMA_SAMPLES_BIT,
}
