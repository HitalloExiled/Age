using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure specifying a descriptor set layout binding</para>
/// <para>The above layout definition allows the descriptor bindings to be specified sparsely such that not all binding numbers between 0 and the maximum binding number need to be specified in the pBindings array. Bindings that are not specified have a descriptorCount and stageFlags of zero, and the value of descriptorType is undefined. However, all binding numbers between 0 and the maximum binding number in the <see cref="VkDescriptorSetLayoutCreateInfo.pBindings"/> array may consume memory in the descriptor set layout even if not all descriptor bindings are used, though it should not consume additional memory from the descriptor pool.</para>
/// <remarks>Note: The maximum binding number specified should be as compact as possible to avoid wasted memory.</remarks>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkDescriptorSetLayoutBinding
{
    /// <summary>
    /// The binding number of this entry and corresponds to a resource of the same binding number in the shader stages.
    /// </summary>
    public uint binding;

    /// <summary>
    /// T <see cref="VkDescriptorType"/> specifying which type of resource descriptors are used for this binding.
    /// </summary>
    public VkDescriptorType descriptorType;

    /// <summary>
    /// The number of descriptors contained in the binding, accessed in a shader as an array, except if descriptorType is <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK"/> in which case descriptorCount is the size in bytes of the inline uniform block. If descriptorCount is zero this binding entry is reserved and the resource must not be accessed from any stage via this binding within any pipeline using the set layout.
    /// </summary>
    public uint descriptorCount;

    /// <summary>
    /// <para>Is a bitmask of <see cref="VkShaderStageFlagBits"/> specifying which pipeline shader stages can access a resource for this binding. <see cref="VK_SHADER_STAGE_ALL"/> is a shorthand specifying that all defined shader stages, including any additional stages defined by extensions, can access the resource.</para>
    /// <para>If a shader stage is not included in stageFlags, then a resource must not be accessed from that stage via this binding within any pipeline using the set layout. Other than input attachments which are limited to the fragment shader, there are no limitations on what combinations of stages can use a descriptor binding, and in particular a binding can be used by both graphics stages and the compute stage.</para>
    /// </summary>
    public VkShaderStageFlags stageFlags;

    /// <summary>
    /// Affects initialization of samplers. If descriptorType specifies a <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_SAMPLER"/> or <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER"/> type descriptor, then pImmutableSamplers can be used to initialize a set of immutable samplers. Immutable samplers are permanently bound into the set layout and must not be changed; updating a <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_SAMPLER"/> descriptor with immutable samplers is not allowed and updates to a <see cref="VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER"/> descriptor with immutable samplers does not modify the samplers (the image views are updated, but the sampler updates are ignored). If pImmutableSamplers is not NULL, then it is a pointer to an array of sampler handles that will be copied into the set layout and used for the corresponding binding. Only the sampler handles are copied; the sampler objects must not be destroyed before the final use of the set layout and any descriptor pools and sets created using it. If pImmutableSamplers is NULL, then the sampler slots are dynamic and sampler handles must be bound into descriptor sets using this layout. If descriptorType is not one of these descriptor types, then pImmutableSamplers is ignored.
    /// </summary>
    public VkSampler* pImmutableSamplers;
}
