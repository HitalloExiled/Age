using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure specifying parameters of a newly created pipeline multisample state.</para>
/// <para>Each bit in the sample mask is associated with a unique sample index as defined for the coverage mask. Each bit b for mask word w in the sample mask corresponds to sample index i, where i = 32 × w + b. pSampleMask has a length equal to ⌈ rasterizationSamples / 32 ⌉ words.</para>
/// <para>If pSampleMask is NULL, it is treated as if the mask has all bits set to 1.</para>
/// </summary>
public unsafe struct VkPipelineMultisampleStateCreateInfo
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
    /// Reserved for future use.
    /// </summary>
    public VkPipelineMultisampleStateCreateFlags flags;

    /// <summary>
    /// A VkSampleCountFlagBits value specifying the number of samples used in rasterization. This value is ignored for the purposes of setting the number of samples used in rasterization if the pipeline is created with the <see cref="VkDynamicState.VK_DYNAMIC_STATE_RASTERIZATION_SAMPLES_EXT"/> dynamic state set, but if <see cref="VkDynamicState.VK_DYNAMIC_STATE_SAMPLE_MASK_EXT"/> dynamic state is not set, it is still used to define the size of the pSampleMask array as described below.
    /// </summary>
    public VkSampleCountFlagBits rasterizationSamples;

    /// <summary>
    /// Can be used to enable Sample Shading.
    /// </summary>
    public VkBool32 sampleShadingEnable;

    /// <summary>
    /// Specifies a minimum fraction of sample shading if sampleShadingEnable is set to VK_TRUE.
    /// </summary>
    public float minSampleShading;

    /// <summary>
    /// A pointer to an array of <see cref="VkSampleMask"/> values used in the sample mask test.
    /// </summary>
    public VkSampleMask* pSampleMask;

    /// <summary>
    /// Controls whether a temporary coverage value is generated based on the alpha component of the fragment’s first color output as specified in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#fragops-covg">Multisample Coverage</see> section.
    /// </summary>
    public VkBool32 alphaToCoverageEnable;

    /// <summary>
    /// Controls whether the alpha component of the fragment’s first color output is replaced with one as described in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#fragops-covg">Multisample Coverage</see>.
    /// </summary>
    public VkBool32 alphaToOneEnable;

    public VkPipelineMultisampleStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO;
}
