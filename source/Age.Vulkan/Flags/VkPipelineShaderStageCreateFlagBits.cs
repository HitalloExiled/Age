namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask controlling how a pipeline shader stage is created.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkPipelineShaderStageCreateFlagBits
{
    /// <summary>
    /// VK_PIPELINE_SHADER_STAGE_CREATE_ALLOW_VARYING_SUBGROUP_SIZE_BIT specifies that the SubgroupSize may vary in the shader stage.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_PIPELINE_SHADER_STAGE_CREATE_ALLOW_VARYING_SUBGROUP_SIZE_BIT = 0x00000001,

    /// <summary>
    /// VK_PIPELINE_SHADER_STAGE_CREATE_REQUIRE_FULL_SUBGROUPS_BIT specifies that the subgroup sizes must be launched with all invocations active in the task, mesh, or compute stage.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_PIPELINE_SHADER_STAGE_CREATE_REQUIRE_FULL_SUBGROUPS_BIT = 0x00000002,

    /// <inheritdoc cref="VK_PIPELINE_SHADER_STAGE_CREATE_ALLOW_VARYING_SUBGROUP_SIZE_BIT" />
    /// <remarks>Provided by VK_EXT_subgroup_size_control</remarks>
    VK_PIPELINE_SHADER_STAGE_CREATE_ALLOW_VARYING_SUBGROUP_SIZE_BIT_EXT = VK_PIPELINE_SHADER_STAGE_CREATE_ALLOW_VARYING_SUBGROUP_SIZE_BIT,

    /// <inheritdoc cref="VK_PIPELINE_SHADER_STAGE_CREATE_REQUIRE_FULL_SUBGROUPS_BIT" />
    /// <remarks>Provided by VK_EXT_subgroup_size_control</remarks>
    VK_PIPELINE_SHADER_STAGE_CREATE_REQUIRE_FULL_SUBGROUPS_BIT_EXT = VK_PIPELINE_SHADER_STAGE_CREATE_REQUIRE_FULL_SUBGROUPS_BIT,
}
