namespace Age.Vulkan.Native.Enums;

public enum VkDescriptorType
{
    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-sampler">sampler descriptor</see>.
    /// </summary>
    VK_DESCRIPTOR_TYPE_SAMPLER = 0,

    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-combinedimagesampler">combined image sampler descriptor</see>.
    /// </summary>
    VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER = 1,

    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-sampledimage">sampled image descriptor</see>.
    /// </summary>
    VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE = 2,

    /// <summary>
    /// Specifies a storage image descriptor.
    /// </summary>
    VK_DESCRIPTOR_TYPE_STORAGE_IMAGE = 3,

    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-uniformtexelbuffer">uniform texel buffer descriptor</see>.
    /// </summary>
    VK_DESCRIPTOR_TYPE_UNIFORM_TEXEL_BUFFER = 4,

    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storagetexelbuffer">storage texel buffer descriptor</see>.
    /// </summary>
    VK_DESCRIPTOR_TYPE_STORAGE_TEXEL_BUFFER = 5,

    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-uniformbuffer">uniform buffer descriptor</see>.
    /// </summary>
    VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER = 6,

    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storagebuffer">storage buffer descriptor</see>.
    /// </summary>
    VK_DESCRIPTOR_TYPE_STORAGE_BUFFER = 7,

    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-uniformbufferdynamic">dynamic uniform buffer descriptor</see>.
    /// </summary>
    VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC = 8,

    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storagebufferdynamic">dynamic storage buffer descriptor</see>.
    /// </summary>
    VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC = 9,

    /// <summary>
    /// Specifies an <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-inputattachment">input attachment descriptor</see>.
    /// </summary>
    VK_DESCRIPTOR_TYPE_INPUT_ATTACHMENT = 10,

    /// <summary>
    /// Specifies an <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-inlineuniformblock">inline uniform block</see>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK = 1000138000,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_DESCRIPTOR_TYPE_ACCELERATION_STRUCTURE_KHR = 1000150000,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_DESCRIPTOR_TYPE_ACCELERATION_STRUCTURE_NV = 1000165000,

    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-weightimage">sampled weight image descriptor</see>.
    /// </summary>
    /// <remarks>Provided by VK_QCOM_image_processing</remarks>
    VK_DESCRIPTOR_TYPE_SAMPLE_WEIGHT_IMAGE_QCOM = 1000440000,

    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-blockmatch">block matching image descriptor</see>.
    /// </summary>
    /// <remarks>Provided by VK_QCOM_image_processing</remarks>
    VK_DESCRIPTOR_TYPE_BLOCK_MATCH_IMAGE_QCOM = 1000440001,

    /// <summary>
    /// Specifies a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-mutable">descriptor of mutable type</see>.
    /// </summary>
    /// <remarks>Provided by VK_EXT_mutable_descriptor_type</remarks>
    VK_DESCRIPTOR_TYPE_MUTABLE_EXT = 1000351000,

    /// <inheritdoc cref="VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK" />
    /// <remarks>Provided by VK_EXT_inline_uniform_block</remarks>
    VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK_EXT = VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK,

    /// <inheritdoc cref="VK_DESCRIPTOR_TYPE_MUTABLE_EXT" />
    /// <remarks>Provided by VK_VALVE_mutable_descriptor_type</remarks>
    VK_DESCRIPTOR_TYPE_MUTABLE_VALVE = VK_DESCRIPTOR_TYPE_MUTABLE_EXT,
}
