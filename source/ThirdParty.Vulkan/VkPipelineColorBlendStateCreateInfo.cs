using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineColorBlendStateCreateInfo.html">VkPipelineColorBlendStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineColorBlendStateCreateInfo
{
    public readonly VkStructureType SType;

    public void*                                PNext;
    public VkPipelineColorBlendStateCreateFlags Flags;
    public VkBool32                             LogicOpEnable;
    public VkLogicOp                            LogicOp;
    public uint                                 AttachmentCount;
    public VkPipelineColorBlendAttachmentState* PAttachments;
    public fixed float                          BlendConstants[4];

    public VkPipelineColorBlendStateCreateInfo() =>
        this.SType = VkStructureType.PipelineColorBlendStateCreateInfo;
}
