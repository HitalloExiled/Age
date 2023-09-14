using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure reporting implementation-dependent physical device limits.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkPhysicalDeviceLimits
{
    /// <summary>
    /// The largest dimension (width) that is guaranteed to be supported for all images created with an imageType of VK_IMAGE_TYPE_1D. Some combinations of image parameters (format, usage, etc.) may allow support for larger dimensions, which can be queried using <see cref="Vk.GetPhysicalDeviceImageFormatProperties"/>.
    /// </summary>
    public uint maxImageDimension1D;

    /// <summary>
    /// The largest dimension (width or height) that is guaranteed to be supported for all images created with an imageType of VK_IMAGE_TYPE_2D and without VK_IMAGE_CREATE_CUBE_COMPATIBLE_BIT set in flags. Some combinations of image parameters (format, usage, etc.) may allow support for larger dimensions, which can be queried using <see cref="Vk.GetPhysicalDeviceImageFormatProperties"/>.
    /// </summary>
    public uint maxImageDimension2D;

    /// <summary>
    /// The largest dimension (width, height, or depth) that is guaranteed to be supported for all images created with an imageType of VK_IMAGE_TYPE_3D. Some combinations of image parameters (format, usage, etc.) may allow support for larger dimensions, which can be queried using <see cref="Vk.GetPhysicalDeviceImageFormatProperties"/>.
    /// </summary>
    public uint maxImageDimension3D;

    /// <summary>
    /// The largest dimension (width or height) that is guaranteed to be supported for all images created with an imageType of VK_IMAGE_TYPE_2D and with VK_IMAGE_CREATE_CUBE_COMPATIBLE_BIT set in flags. Some combinations of image parameters (format, usage, etc.) may allow support for larger dimensions, which can be queried using <see cref="Vk.GetPhysicalDeviceImageFormatProperties"/>.
    /// </summary>
    public uint maxImageDimensionCube;

    /// <summary>
    /// The maximum number of layers (arrayLayers) for an image.
    /// </summary>
    public uint maxImageArrayLayers;

    /// <summary>
    /// The maximum number of addressable texels for a buffer view created on a buffer which was created with the <see cref="VkBufferCreateInfo.VK_BUFFER_USAGE_UNIFORM_TEXEL_BUFFER_BIT"/> or <see cref="VkBufferCreateInfo.VK_BUFFER_USAGE_STORAGE_TEXEL_BUFFER_BIT"/> set in the usage member of the <see cref="VkBufferCreateInfo"/> structure.
    /// </summary>
    public uint maxTexelBufferElements;

    /// <summary>
    /// The maximum value that can be specified in the range member of a <see cref="VkDescriptorBufferInfo"/> structure passed to <see cref="Vk.UpdateDescriptorSets"/> for descriptors of type <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC"/>.
    /// </summary>
    public uint maxUniformBufferRange;

    /// <summary>
    /// The maximum value that can be specified in the range member of a <see cref="VkDescriptorBufferInfo"/> structure passed to <see cref="Vk.UpdateDescriptorSets"/> for descriptors of type <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC"/>.
    /// </summary>
    public uint maxStorageBufferRange;

    /// <summary>
    /// The maximum size, in bytes, of the pool of push constant memory. For each of the push constant ranges indicated by the <see cref="pPushConstantRanges"/> member of the <see cref="VkPipelineLayoutCreateInfo"/> structure, (offset + size) must be less than or equal to this limit.
    /// </summary>
    public uint maxPushConstantsSize;

    /// <summary>
    /// The maximum number of device memory allocations, as created by <see cref="Vk.AllocateMemory"/>, which can simultaneously exist.
    /// </summary>
    public uint maxMemoryAllocationCount;

    /// <summary>
    /// The maximum number of sampler objects, as created by <see cref="Vk.CreateSampler"/>, which can simultaneously exist on a device.
    /// </summary>
    public uint maxSamplerAllocationCount;

    /// <summary>
    /// The granularity, in bytes, at which buffer or linear image resources, and optimal image resources can be bound to adjacent offsets in the same <see cref="VkDeviceMemory"/> object without aliasing. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#resources-bufferimagegranularity">Buffer-Image Granularity</see> for more details.
    /// </summary>
    public VkDeviceSize bufferImageGranularity;

    /// <summary>
    /// The total amount of address space available, in bytes, for sparse memory resources. This is an upper bound on the sum of the sizes of all sparse resources, regardless of whether any memory is bound to them.
    /// </summary>
    public VkDeviceSize sparseAddressSpaceSize;

    /// <summary>
    /// The maximum number of descriptor sets that can be simultaneously used by a pipeline. All DescriptorSet decorations in shader modules must have a value less than maxBoundDescriptorSets. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-sets.
    /// </summary>
    public uint maxBoundDescriptorSets;

    /// <summary>
    /// The maximum number of samplers that can be accessible to a single shader stage in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_SAMPLER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. A descriptor is accessible to a shader stage when the stageFlags member of the <see cref="VkDescriptorSetLayoutBinding"/> structure has the bit for that shader stage set. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-sampler and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-combinedimagesampler.
    /// </summary>
    public uint maxPerStageDescriptorSamplers;

    /// <summary>
    /// The maximum number of uniform buffers that can be accessible to a single shader stage in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. A descriptor is accessible to a shader stage when the stageFlags member of the <see cref="VkDescriptorSetLayoutBinding"/> structure has the bit for that shader stage set. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-uniformbuffer and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-uniformbufferdynamic.
    /// </summary>
    public uint maxPerStageDescriptorUniformBuffers;

    /// <summary>
    /// The maximum number of storage buffers that can be accessible to a single shader stage in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. A descriptor is accessible to a pipeline shader stage when the stageFlags member of the <see cref="VkDescriptorSetLayoutBinding"/> structure has the bit for that shader stage set. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storagebuffer and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storagebufferdynamic.
    /// </summary>
    public uint maxPerStageDescriptorStorageBuffers;

    /// <summary>
    /// The maximum number of sampled images that can be accessible to a single shader stage in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER"/>, <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE"/>, or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_TEXEL_BUFFER"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. A descriptor is accessible to a pipeline shader stage when the stageFlags member of the <see cref="VkDescriptorSetLayoutBinding"/> structure has the bit for that shader stage set. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-combinedimagesampler, https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-sampledimage, and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-uniformtexelbuffer.
    /// </summary>
    public uint maxPerStageDescriptorSampledImages;

    /// <summary>
    /// The maximum number of storage images that can be accessible to a single shader stage in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_IMAGE"/>, or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_TEXEL_BUFFER"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. A descriptor is accessible to a pipeline shader stage when the stageFlags member of the <see cref="VkDescriptorSetLayoutBinding"/> structure has the bit for that shader stage set. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storageimage, and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storagetexelbuffer.
    /// </summary>
    public uint maxPerStageDescriptorStorageImages;

    /// <summary>
    /// The maximum number of input attachments that can be accessible to a single shader stage in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INPUT_ATTACHMENT"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. A descriptor is accessible to a pipeline shader stage when the stageFlags member of the <see cref="VkDescriptorSetLayoutBinding"/> structure has the bit for that shader stage set. These are only supported for the fragment stage. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-inputattachment.
    /// </summary>
    public uint maxPerStageDescriptorInputAttachments;

    /// <summary>
    /// The maximum number of resources that can be accessible to a single shader stage in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER"/>, <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE"/>, <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_IMAGE"/>, <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_TEXEL_BUFFER"/>, <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_TEXEL_BUFFER"/>, <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER"/>, <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER"/>, <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC"/>, <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC"/>, or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INPUT_ATTACHMENT"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. For the fragment shader stage the framebuffer color attachments also count against this limit.
    /// </summary>
    public uint maxPerStageResources;

    /// <summary>
    /// The maximum number of samplers that can be included in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_SAMPLER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-sampler and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-combinedimagesampler.
    /// </summary>
    public uint maxDescriptorSetSamplers;

    /// <summary>
    /// The maximum number of uniform buffers that can be included in a pipeline layout. Descriptors with a type of VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER or VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC count against this limit. Only descriptors in descriptor set layouts created without the VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT bit set count against this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-uniformbuffer and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-uniformbufferdynamic.
    /// </summary>
    public uint maxDescriptorSetUniformBuffers;

    /// <summary>
    /// The maximum number of dynamic uniform buffers that can be included in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-uniformbufferdynamic.
    /// </summary>
    public uint maxDescriptorSetUniformBuffersDynamic;

    /// <summary>
    /// The maximum number of storage buffers that can be included in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storagebuffer and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storagebufferdynamic.
    /// </summary>
    public uint maxDescriptorSetStorageBuffers;

    /// <summary>
    /// The maximum number of dynamic storage buffers that can be included in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storagebufferdynamic.
    /// </summary>
    public uint maxDescriptorSetStorageBuffersDynamic;

    /// <summary>
    /// The maximum number of sampled images that can be included in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER"/>, <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE"/>, or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_TEXEL_BUFFER"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-combinedimagesampler, https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-sampledimage, and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-uniformtexelbuffer.
    /// </summary>
    public uint maxDescriptorSetSampledImages;

    /// <summary>
    /// The maximum number of storage images that can be included in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_IMAGE"/>, or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_TEXEL_BUFFER"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storageimage, and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-storagetexelbuffer.
    /// </summary>
    public uint maxDescriptorSetStorageImages;

    /// <summary>
    /// The maximum number of input attachments that can be included in a pipeline layout. Descriptors with a type of <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INPUT_ATTACHMENT"/> count against this limit. Only descriptors in descriptor set layouts created without the <see cref="VK_DESCRIPTOR_SET_LAYOUT_CREATE_UPDATE_AFTER_BIND_POOL_BIT"/> bit set count against this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-inputattachment.
    /// </summary>
    public uint maxDescriptorSetInputAttachments;

    /// <summary>
    /// The maximum number of vertex input attributes that can be specified for a graphics pipeline. These are described in the array of <see cref="VkVertexInputAttributeDescription"/> structures that are provided at graphics pipeline creation time via the <see cref="pVertexAttributeDescriptions"/> member of the <see cref="VkPipelineVertexInputStateCreateInfo"/> structure. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#fxvertex-attrib and https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#fxvertex-input.
    /// </summary>
    public uint maxVertexInputAttributes;

    /// <summary>
    /// The maximum number of vertex buffers that can be specified for providing vertex attributes to a graphics pipeline. These are described in the array of <see cref="VkVertexInputBindingDescription"/> structures that are provided at graphics pipeline creation time via the <see cref="pVertexBindingDescriptions"/> member of the <see cref="VkPipelineVertexInputStateCreateInfo"/> structure. The binding member of <see cref="VkVertexInputBindingDescription"/> must be less than this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#fxvertex-input.
    /// </summary>
    public uint maxVertexInputBindings;

    /// <summary>
    /// The maximum vertex input attribute offset that can be added to the vertex input binding stride. The offset member of the <see cref="VkVertexInputAttributeDescription"/> structure must be less than or equal to this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#fxvertex-input.
    /// </summary>
    public uint maxVertexInputAttributeOffset;

    /// <summary>
    /// The maximum vertex input binding stride that can be specified in a vertex input binding. The stride member of the <see cref="VkVertexInputBindingDescription"/> structure must be less than or equal to this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#fxvertex-input.
    /// </summary>
    public uint maxVertexInputBindingStride;

    /// <summary>
    /// The maximum number of components of output variables which can be output by a vertex shader. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#shaders-vertex.
    /// </summary>
    public uint maxVertexOutputComponents;

    /// <summary>
    /// The maximum tessellation generation level supported by the fixed-function tessellation primitive generator. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#tessellation.
    /// </summary>
    public uint maxTessellationGenerationLevel;

    /// <summary>
    /// The maximum patch size, in vertices, of patches that can be processed by the tessellation control shader and tessellation primitive generator. The patchControlPoints member of the <see cref="VkPipelineTessellationStateCreateInfo"/> structure specified at pipeline creation time and the value provided in the OutputVertices execution mode of shader modules must be less than or equal to this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#tessellation.
    /// </summary>
    public uint maxTessellationPatchSize;

    /// <summary>
    /// The maximum number of components of input variables which can be provided as per-vertex inputs to the tessellation control shader stage.
    /// </summary>
    public uint maxTessellationControlPerVertexInputComponents;

    /// <summary>
    /// The maximum number of components of per-vertex output variables which can be output from the tessellation control shader stage.
    /// </summary>
    public uint maxTessellationControlPerVertexOutputComponents;

    /// <summary>
    /// The maximum number of components of per-patch output variables which can be output from the tessellation control shader stage.
    /// </summary>
    public uint maxTessellationControlPerPatchOutputComponents;

    /// <summary>
    /// The maximum total number of components of per-vertex and per-patch output variables which can be output from the tessellation control shader stage.
    /// </summary>
    public uint maxTessellationControlTotalOutputComponents;

    /// <summary>
    /// The maximum number of components of input variables which can be provided as per-vertex inputs to the tessellation evaluation shader stage.
    /// </summary>
    public uint maxTessellationEvaluationInputComponents;

    /// <summary>
    /// The maximum number of components of per-vertex output variables which can be output from the tessellation evaluation shader stage.
    /// </summary>
    public uint maxTessellationEvaluationOutputComponents;

    /// <summary>
    /// The maximum invocation count supported for instanced geometry shaders. The value provided in the Invocations execution mode of shader modules must be less than or equal to this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#geometry.
    /// </summary>
    public uint maxGeometryShaderInvocations;

    /// <summary>
    /// The maximum number of components of input variables which can be provided as inputs to the geometry shader stage.
    /// </summary>
    public uint maxGeometryInputComponents;

    /// <summary>
    /// The maximum number of components of output variables which can be output from the geometry shader stage.
    /// </summary>
    public uint maxGeometryOutputComponents;

    /// <summary>
    /// The maximum number of vertices which can be emitted by any geometry shader.
    /// </summary>
    public uint maxGeometryOutputVertices;

    /// <summary>
    /// The maximum total number of components of output variables, across all emitted vertices, which can be output from the geometry shader stage.
    /// </summary>
    public uint maxGeometryTotalOutputComponents;

    /// <summary>
    /// The maximum number of components of input variables which can be provided as inputs to the fragment shader stage.
    /// </summary>
    public uint maxFragmentInputComponents;

    /// <summary>
    /// The maximum number of output attachments which can be written to by the fragment shader stage.
    /// </summary>
    public uint maxFragmentOutputAttachments;

    /// <summary>
    /// The maximum number of output attachments which can be written to by the fragment shader stage when blending is enabled and one of the dual source blend modes is in use. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#framebuffer-dsb and dualSrcBlend.
    /// </summary>
    public uint maxFragmentDualSrcAttachments;

    /// <summary>
    /// The total number of storage buffers, storage images, and output Location decorated color attachments (described in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#interfaces-fragmentoutput">Fragment Output Interface</see>) which can be used in the fragment shader stage.
    /// </summary>
    public uint maxFragmentCombinedOutputResources;

    /// <summary>
    /// The maximum total storage size, in bytes, available for variables declared with the Workgroup storage class in shader modules (or with the shared storage qualifier in GLSL) in the compute shader stage.
    /// </summary>
    public uint maxComputeSharedMemorySize;

    /// <summary>
    /// The maximum number of local workgroups that can be dispatched by a single dispatching command. These three values represent the maximum number of local workgroups for the X, Y, and Z dimensions, respectively. The workgroup count parameters to the dispatching commands must be less than or equal to the corresponding limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#dispatch.
    /// </summary>
    public fixed uint maxComputeWorkGroupCount[3];

    /// <summary>
    /// The maximum total number of compute shader invocations in a single local workgroup. The product of the X, Y, and Z sizes, as specified by the LocalSize or LocalSizeId execution mode in shader modules or by the object decorated by the WorkgroupSize decoration, must be less than or equal to this limit.
    /// </summary>
    public uint maxComputeWorkGroupInvocations;

    /// <summary>
    /// The maximum size of a local compute workgroup, per dimension. These three values represent the maximum local workgroup size in the X, Y, and Z dimensions, respectively. The x, y, and z sizes, as specified by the LocalSize or LocalSizeId execution mode or by the object decorated by the WorkgroupSize decoration in shader modules, must be less than or equal to the corresponding limit.
    /// </summary>
    public fixed uint maxComputeWorkGroupSize[3];

    /// <summary>
    /// The number of bits of subpixel precision in framebuffer coordinates xf and yf. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#primsrast.
    /// </summary>
    public uint subPixelPrecisionBits;

    /// <summary>
    /// The number of bits of precision in the division along an axis of an image used for minification and magnification filters. 2subTexelPrecisionBits is the actual number of divisions along each axis of the image represented. Sub-texel values calculated during image sampling will snap to these locations when generating the filtered results.
    /// </summary>
    public uint subTexelPrecisionBits;

    /// <summary>
    /// The number of bits of division that the LOD calculation for mipmap fetching get snapped to when determining the contribution from each mip level to the mip filtered results. 2mipmapPrecisionBits is the actual number of divisions.
    /// </summary>
    public uint mipmapPrecisionBits;

    /// <summary>
    /// The maximum index value that can be used for indexed draw calls when using 32-bit indices. This excludes the primitive restart index value of 0xFFFFFFFF. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-fullDrawIndexUint32">fullDrawIndexUint32</see>.
    /// </summary>
    public uint maxDrawIndexedIndexValue;

    /// <summary>
    /// The maximum draw count that is supported for indirect drawing calls. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-multiDrawIndirect">multiDrawIndirect</see>.
    /// </summary>
    public uint maxDrawIndirectCount;

    /// <summary>
    /// The maximum absolute sampler LOD bias. The sum of the mipLodBias member of the VkSamplerCreateInfo structure and the Bias operand of image sampling operations in shader modules (or 0 if no Bias operand is provided to an image sampling operation) are clamped to the range [-maxSamplerLodBias,+maxSamplerLodBias]. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#samplers-mipLodBias.
    /// </summary>
    public float maxSamplerLodBias;

    /// <summary>
    /// The maximum degree of sampler anisotropy. The maximum degree of anisotropic filtering used for an image sampling operation is the minimum of the maxAnisotropy member of the <see cref="VkSamplerCreateInfo"/> structure and this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#samplers-maxAnisotropy.
    /// </summary>
    public float maxSamplerAnisotropy;

    /// <summary>
    /// The maximum number of active viewports. The viewportCount member of the <see cref="VkPipelineViewportStateCreateInfo"/> structure that is provided at pipeline creation must be less than or equal to this limit.
    /// </summary>
    public uint maxViewports;

    /// <summary>
    /// The maximum viewport dimensions in the X (width) and Y (height) dimensions, respectively. The maximum viewport dimensions must be greater than or equal to the largest image which can be created and used as a framebuffer attachment. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#vertexpostproc-viewport">Controlling the Viewport</see>.
    /// </summary>
    public fixed uint maxViewportDimensions[2];

    /// <summary>
    /// The [minimum, maximum] range that the corners of a viewport must be contained in. This range must be at least [-2 × size, 2 × size - 1], where size = max(maxViewportDimensions[0], maxViewportDimensions[1]). See Controlling the Viewport.
    /// </summary>
    /// <remarks>The intent of the viewportBoundsRange limit is to allow a maximum sized viewport to be arbitrarily shifted relative to the output target as long as at least some portion intersects. This would give a bounds limit of [-size + 1, 2 × size - 1] which would allow all possible non-empty-set intersections of the output target and the viewport. Since these numbers are typically powers of two, picking the signed number range using the smallest possible number of bits ends up with the specified range.</remarks>
    public fixed float viewportBoundsRange[2];

    /// <summary>
    /// The number of bits of subpixel precision for viewport bounds. The subpixel precision that floating-point viewport bounds are interpreted at is given by this limit.
    /// </summary>
    public uint viewportSubPixelBits;

    /// <summary>
    /// The minimum required alignment, in bytes, of host visible memory allocations within the host address space. When mapping a memory allocation with <see cref="Vk.MapMemory"/>, subtracting offset bytes from the returned pointer will always produce an integer multiple of this limit. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-device-hostaccess. The value must be a power of two.
    /// </summary>
    public int minMemoryMapAlignment;

    /// <summary>
    /// The minimum required alignment, in bytes, for the offset member of the <see cref="VkBufferViewCreateInfo"/> structure for texel buffers. The value must be a power of two. If <see cref="texelBufferAlignment"/> is enabled, this limit is equivalent to the maximum of the <see cref="uniformTexelBufferOffsetAlignmentBytes"/> and <see cref="storageTexelBufferOffsetAlignmentBytes"/> members of <see cref="VkPhysicalDeviceTexelBufferAlignmentProperties"/>, but smaller alignment is optionally allowed by <see cref="storageTexelBufferOffsetSingleTexelAlignment"/> and <see cref="uniformTexelBufferOffsetSingleTexelAlignment"/>. If <see cref="texelBufferAlignment"/> is not enabled, <see cref="VkBufferViewCreateInfo.offset"/> must be a multiple of this value.
    /// </summary>
    public VkDeviceSize minTexelBufferOffsetAlignment;

    /// <summary>
    /// The minimum required alignment, in bytes, for the offset member of the <see cref="VkDescriptorBufferInfo"/> structure for uniform buffers. When a descriptor of type <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC"/> is updated, the offset must be an integer multiple of this limit. Similarly, dynamic offsets for uniform buffers must be multiples of this limit. The value must be a power of two.
    /// </summary>
    public VkDeviceSize minUniformBufferOffsetAlignment;

    /// <summary>
    /// The minimum required alignment, in bytes, for the offset member of the <see cref="VkDescriptorBufferInfo"/> structure for storage buffers. When a descriptor of type <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC"/> is updated, the offset must be an integer multiple of this limit. Similarly, dynamic offsets for storage buffers must be multiples of this limit. The value must be a power of two.
    /// </summary>
    public VkDeviceSize minStorageBufferOffsetAlignment;

    /// <summary>
    /// The minimum offset value for the ConstOffset image operand of any of the OpImageSample* or OpImageFetch* image instructions.
    /// </summary>
    public int minTexelOffset;

    /// <summary>
    /// The maximum offset value for the ConstOffset image operand of any of the OpImageSample* or OpImageFetch* image instructions.
    /// </summary>
    public uint maxTexelOffset;

    /// <summary>
    /// The minimum offset value for the Offset, ConstOffset, or ConstOffsets image operands of any of the OpImage*Gather image instructions.
    /// </summary>
    public int minTexelGatherOffset;

    /// <summary>
    /// The maximum offset value for the Offset, ConstOffset, or ConstOffsets image operands of any of the OpImage*Gather image instructions.
    /// </summary>
    public uint maxTexelGatherOffset;

    /// <summary>
    /// The base minimum (inclusive) negative offset value for the Offset operand of the InterpolateAtOffset extended instruction.
    /// </summary>
    public float minInterpolationOffset;

    /// <summary>
    /// The base maximum (inclusive) positive offset value for the Offset operand of the InterpolateAtOffset extended instruction.
    /// </summary>
    public float maxInterpolationOffset;

    /// <summary>
    /// The number of fractional bits that the x and y offsets to the InterpolateAtOffset extended instruction may be rounded to as fixed-point values.
    /// </summary>
    public uint subPixelInterpolationOffsetBits;

    /// <summary>
    /// The maximum width for a framebuffer. The width member of the VkFramebufferCreateInfo structure must be less than or equal to this limit.
    /// </summary>
    public uint maxFramebufferWidth;

    /// <summary>
    /// The maximum height for a framebuffer. The height member of the VkFramebufferCreateInfo structure must be less than or equal to this limit.
    /// </summary>
    public uint maxFramebufferHeight;

    /// <summary>
    /// The maximum layer count for a layered framebuffer. The layers member of the VkFramebufferCreateInfo structure must be less than or equal to this limit.
    /// </summary>
    public uint maxFramebufferLayers;

    /// <summary>
    /// A bitmask1 of <see cref="VkSampleCountFlagBits"/> indicating the color sample counts that are supported for all framebuffer color attachments with floating- or fixed-point formats. For color attachments with integer formats, see <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#limits-framebufferIntegerColorSampleCounts">framebufferIntegerColorSampleCounts</see>.
    /// </summary>
    public VkSampleCountFlags framebufferColorSampleCounts;

    /// <summary>
    /// A bitmask1 of <see cref="VkSampleCountFlagBits"/> indicating the supported depth sample counts for all framebuffer depth/stencil attachments, when the format includes a depth component.
    /// </summary>
    public VkSampleCountFlags framebufferDepthSampleCounts;

    /// <summary>
    /// A bitmask1 of <see cref="VkSampleCountFlagBits"/> indicating the supported stencil sample counts for all framebuffer depth/stencil attachments, when the format includes a stencil component.
    /// </summary>
    public VkSampleCountFlags framebufferStencilSampleCounts;

    /// <summary>
    /// A bitmask1 of <see cref="VkSampleCountFlagBits"/> indicating the supported sample counts for a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#renderpass-noattachments">subpass which uses no attachments</see>.
    /// </summary>
    public VkSampleCountFlags framebufferNoAttachmentsSampleCounts;

    /// <summary>
    /// The maximum number of color attachments that can be used by a subpass in a render pass. The colorAttachmentCount member of the VkSubpassDescription or VkSubpassDescription2 structure must be less than or equal to this limit.
    /// </summary>
    public uint maxColorAttachments;

    /// <summary>
    /// A bitmask1 of <see cref="VkSampleCountFlagBits"/> indicating the sample counts supported for all 2D images created with <see cref="VK_IMAGE_TILING_OPTIMAL"/>, usage containing <see cref="VK_IMAGE_USAGE_SAMPLED_BIT"/>, and a non-integer color format.
    /// </summary>
    public VkSampleCountFlags sampledImageColorSampleCounts;

    /// <summary>
    /// A bitmask1 of <see cref="VkSampleCountFlagBits"/> indicating the sample counts supported for all 2D images created with <see cref="VK_IMAGE_TILING_OPTIMAL"/>, usage containing <see cref="VK_IMAGE_USAGE_SAMPLED_BIT"/>, and an integer color format.
    /// </summary>
    public VkSampleCountFlags sampledImageIntegerSampleCounts;

    /// <summary>
    /// A bitmask1 of <see cref="VkSampleCountFlagBits"/> indicating the sample counts supported for all 2D images created with <see cref="VK_IMAGE_TILING_OPTIMAL"/>, usage containing <see cref="VK_IMAGE_USAGE_SAMPLED_BIT"/>, and a depth format.
    /// </summary>
    public VkSampleCountFlags sampledImageDepthSampleCounts;

    /// <summary>
    /// A bitmask1 of <see cref="VkSampleCountFlagBits"/> indicating the sample counts supported for all 2D images created with <see cref="VK_IMAGE_TILING_OPTIMAL"/>, usage containing <see cref="VK_IMAGE_USAGE_SAMPLED_BIT"/>, and a stencil format.
    /// </summary>
    public VkSampleCountFlags sampledImageStencilSampleCounts;

    /// <summary>
    /// A bitmask1 of <see cref="VkSampleCountFlagBits"/> indicating the sample counts supported for all 2D images created with <see cref="VK_IMAGE_TILING_OPTIMAL"/>, and usage containing <see cref="VK_IMAGE_USAGE_STORAGE_BIT"/>.
    /// </summary>
    public VkSampleCountFlags storageImageSampleCounts;

    /// <summary>
    /// The maximum number of array elements of a variable decorated with the SampleMask built-in decoration.
    /// </summary>
    public uint maxSampleMaskWords;

    /// <summary>
    /// Specifies support for timestamps on all graphics and compute queues. If this limit is set to true, all queues that advertise the <see cref="VK_QUEUE_GRAPHICS_BIT"/> or <see cref="VK_QUEUE_COMPUTE_BIT"/> in the <see cref="VkQueueFamilyProperties.queueFlags"/> support <see cref="VkQueueFamilyProperties.timestampValidBits"/> of at least 36. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#queries-timestamps">Timestamp Queries</see>.
    /// </summary>
    public VkBool32 timestampComputeAndGraphics;

    /// <summary>
    /// The number of nanoseconds required for a timestamp query to be incremented by 1. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#queries-timestamps">Timestamp Queries</see>.
    /// </summary>
    public float timestampPeriod;

    /// <summary>
    /// The maximum number of clip distances that can be used in a single shader stage. The size of any array declared with the ClipDistance built-in decoration in a shader module must be less than or equal to this limit.
    /// </summary>
    public uint maxClipDistances;

    /// <summary>
    /// The maximum number of cull distances that can be used in a single shader stage. The size of any array declared with the CullDistance built-in decoration in a shader module must be less than or equal to this limit.
    /// </summary>
    public uint maxCullDistances;

    /// <summary>
    /// The maximum combined number of clip and cull distances that can be used in a single shader stage. The sum of the sizes of any pair of arrays declared with the ClipDistance and CullDistance built-in decoration used by a single shader stage in a shader module must be less than or equal to this limit.
    /// </summary>
    public uint maxCombinedClipAndCullDistances;

    /// <summary>
    /// The number of discrete priorities that can be assigned to a queue based on the value of each member of <see cref="VkDeviceQueueCreateInfo.pQueuePriorities"/>. This must be at least 2, and levels must be spread evenly over the range, with at least one level at 1.0, and another at 0.0. See https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#devsandqueues-priority.
    /// </summary>
    public uint discreteQueuePriorities;

    /// <summary>
    /// The range [minimum,maximum] of supported sizes for points. Values written to variables decorated with the PointSize built-in decoration are clamped to this range.
    /// </summary>
    public fixed float pointSizeRange[2];

    /// <summary>
    /// The range [minimum,maximum] of supported widths for lines. Values specified by the lineWidth member of the <see cref="VkPipelineRasterizationStateCreateInfo"/> or the lineWidth parameter to <see cref="Vk.CmdSetLineWidth"/> are clamped to this range.
    /// </summary>
    public fixed float lineWidthRange[2];

    /// <summary>
    /// The granularity of supported point sizes. Not all point sizes in the range defined by <see cref="pointSizeRange"/> are supported. This limit specifies the granularity (or increment) between successive supported point sizes.
    /// </summary>
    public float pointSizeGranularity;

    /// <summary>
    /// The granularity of supported line widths. Not all line widths in the range defined by <see cref="lineWidthRange"/> are supported. This limit specifies the granularity (or increment) between successive supported line widths.
    /// </summary>
    public float lineWidthGranularity;

    /// <summary>
    /// Specifies whether lines are rasterized according to the preferred method of rasterization. If set to false, lines may be rasterized under a relaxed set of rules. If set to true, lines are rasterized as per the strict definition. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#primsrast-lines-basic">Basic Line Segment Rasterization</see>.
    /// </summary>
    public VkBool32 strictLines;

    /// <summary>
    /// Specifies whether rasterization uses the standard sample locations as documented in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#primsrast-multisampling">Multisampling</see>. If set to VK_TRUE, the implementation uses the documented sample locations. If set to VK_FALSE, the implementation may use different sample locations.
    /// </summary>
    public VkBool32 standardSampleLocations;

    /// <summary>
    /// Is the optimal buffer offset alignment in bytes for <see cref="Vk.CmdCopyBufferToImage2"/>, <see cref="Vk.CmdCopyBufferToImage"/>, <see cref="Vk.CmdCopyImageToBuffer2"/>, and <see cref="Vk.CmdCopyImageToBuffer"/>. This value is also the optimal host memory offset alignment in bytes for <see cref="Vk.CopyMemoryToImageEXT"/> and <see cref="VkExtHostImageCopy.CopyImageToMemoryEXT"/>. The per texel alignment requirements are enforced, but applications should use the optimal alignment for optimal performance and power use. The value must be a power of two.
    /// </summary>
    public VkDeviceSize optimalBufferCopyOffsetAlignment;

    /// <summary>
    /// The optimal buffer row pitch alignment in bytes for <see cref="Vk.CmdCopyBufferToImage2"/>, <see cref="Vk.CmdCopyBufferToImage"/>, <see cref="Vk.CmdCopyImageToBuffer2"/>, and <see cref="Vk.CmdCopyImageToBuffer"/>. This value is also the optimal host memory row pitch alignment in bytes for <see cref="VkExtHostImageCopy.CopyMemoryToImageEXT"/> and <see cref="VkExtHostImageCopy.CopyImageToMemoryEXT"/>. Row pitch is the number of bytes between texels with the same X coordinate in adjacent rows (Y coordinates differ by one). The per texel alignment requirements are enforced, but applications should use the optimal alignment for optimal performance and power use. The value must be a power of two.
    /// </summary>
    public VkDeviceSize optimalBufferCopyRowPitchAlignment;

    /// <summary>
    /// The size and alignment in bytes that bounds concurrent access to host-mapped device memory. The value must be a power of two.
    /// </summary>
    /// <remarks>For all bitmasks of <see cref="VkSampleCountFlagBits"/>, the sample count limits defined above represent the minimum supported sample counts for each image type. Individual images may support additional sample counts, which are queried using <see cref="Vk.GetPhysicalDeviceImageFormatProperties"/> as described in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-supported-sample-counts">Supported Sample Counts</see>.</remarks>
    public VkDeviceSize nonCoherentAtomSize;
}
