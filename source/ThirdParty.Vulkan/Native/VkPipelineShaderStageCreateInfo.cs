namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineShaderStageCreateInfo.html">VkPipelineShaderStageCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineShaderStageCreateInfo
{
    public readonly VkStructureType sType;

    public void*                            pNext;
    public VkPipelineShaderStageCreateFlags flags;
    public VkShaderStageFlagBits            stage;
    public VkShaderModule                   module;
    public byte*                            pName;
    public VkSpecializationInfo*            pSpecializationInfo;


    public VkPipelineShaderStageCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO;
}
