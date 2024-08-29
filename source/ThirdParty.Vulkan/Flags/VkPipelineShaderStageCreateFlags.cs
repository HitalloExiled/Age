namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineShaderStageCreateFlagBits.html">VkPipelineShaderStageCreateFlagBits</see>
/// </summary>
[Flags]
public enum VkPipelineShaderStageCreateFlags
{
    AllowVaryingSubgroupSize    = 0x00000001,
    RequireFullSubgroups        = 0x00000002,
    AllowVaryingSubgroupSizeEXT = AllowVaryingSubgroupSize,
    RequireFullSubgroupsEXT     = RequireFullSubgroups,
}
