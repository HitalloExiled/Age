namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineLayoutCreateInfo.html">VkPipelineLayoutCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineLayoutCreateInfo
{
    public readonly VkStructureType sType;

    public void*                       pNext;
    public VkPipelineLayoutCreateFlags flags;
    public uint                        setLayoutCount;
    public VkDescriptorSetLayout*      pSetLayouts;
    public uint                        pushConstantRangeCount;
    public VkPushConstantRange*        pPushConstantRanges;

    public VkPipelineLayoutCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO;
}
