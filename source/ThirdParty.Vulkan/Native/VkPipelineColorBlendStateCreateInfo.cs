namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineColorBlendStateCreateInfo.html">VkPipelineColorBlendStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineColorBlendStateCreateInfo
{
    public readonly VkStructureType sType;

    public void*                                pNext;
    public VkPipelineColorBlendStateCreateFlags flags;
    public VkBool32                             logicOpEnable;
    public VkLogicOp                            logicOp;
    public uint                                 attachmentCount;
    public VkPipelineColorBlendAttachmentState* pAttachments;
    public fixed float                          blendConstants[4];

    public VkPipelineColorBlendStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO;
}
