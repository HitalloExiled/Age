using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying parameters of a newly created pipeline shader stage.</para>
/// <para>If module is not VK_NULL_HANDLE, the shader code used by the pipeline is defined by module. If module is VK_NULL_HANDLE, the shader code is defined by the chained <see cref="VkShaderModuleCreateInfo"/> if present.</para>
/// <para>If the shaderModuleIdentifier feature is enabled, applications can omit shader code for stage and instead provide a module identifier. This is done by including a <see cref="VkPipelineShaderStageModuleIdentifierCreateInfoEXT"/> struct with identifierSize not equal to 0 in the pNext chain. A shader stage created in this way is equivalent to one created using a shader module with the same identifier. The identifier allows an implementation to look up a pipeline without consuming a valid SPIR-V module. If a pipeline is not found, pipeline compilation is not possible and the implementation must fail as specified by<see cref="VkPipelineCreateFlagBits.VK_PIPELINE_CREATE_FAIL_ON_PIPELINE_COMPILE_REQUIRED_BIT"/>.</para>
/// <para>When an identifier is used in lieu of a shader module, implementations may fail pipeline compilation with <see cref="VkResult.VK_PIPELINE_COMPILE_REQUIRED"/> for any reason.</para>
/// <remarks>The rationale for the relaxed requirement on implementations to return a pipeline with <see cref="VkPipelineShaderStageModuleIdentifierCreateInfoEXT"/> is that layers or tools may intercept pipeline creation calls and require the full SPIR-V context to operate correctly. ICDs are not expected to fail pipeline compilation if the pipeline exists in a cache somewhere.</remarks>
/// <para>Applications can use identifiers when creating pipelines with <see cref="VkPipelineCreateFlagBits.VK_PIPELINE_CREATE_LIBRARY_BIT_KHR"/>. When creating such pipelines, <see cref="VkResult.VK_SUCCESS"/> may be returned, but subsequently fail when referencing the pipeline in a <see cref="VkPipelineLibraryCreateInfoKHR"/> struct. Applications must allow pipeline compilation to fail during link steps with <see cref="VkPipelineCreateFlagBits.VK_PIPELINE_CREATE_FAIL_ON_PIPELINE_COMPILE_REQUIRED_BIT"/> as it may not be possible to determine if a pipeline can be created from identifiers until the link step.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkPipelineShaderStageCreateInfo
{

    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public VkStructureType sType;

    /// <summary>
    /// NULL or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask of <see cref="VkPipelineShaderStageCreateFlagBits"/> specifying how the pipeline shader stage will be generated.
    /// </summary>
    public VkPipelineShaderStageCreateFlags flags;

    /// <summary>
    /// A <see cref="VkShaderStageFlagBits"/> value specifying a single pipeline stage.
    /// </summary>
    public VkShaderStageFlagBits stage;

    /// <summary>
    /// Optionally a <see cref="VkShaderModule"/> object containing the shader code for this stage.
    /// </summary>
    public VkShaderModule module;

    /// <summary>
    /// A pointer to a null-terminated UTF-8 string specifying the entry point name of the shader for this stage.
    /// </summary>
    public byte* pName;

    /// <summary>
    /// A pointer to a <see cref="VkSpecializationInfo"/> structure, as described in Specialization Constants, or NULL.
    /// </summary>
    public VkSpecializationInfo* pSpecializationInfo;


    public VkPipelineShaderStageCreateInfo() =>
        sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO;
}
