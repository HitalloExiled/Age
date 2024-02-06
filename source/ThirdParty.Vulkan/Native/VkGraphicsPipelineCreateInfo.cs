using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkGraphicsPipelineCreateInfo.html">VkGraphicsPipelineCreateInfo</see>
/// </summary>
public unsafe struct VkGraphicsPipelineCreateInfo
{
    public readonly VkStructureType SType;

    public void*                                   PNext;
    public VkPipelineCreateFlags                   Flags;
    public uint                                    StageCount;
    public VkPipelineShaderStageCreateInfo*        PStages;
    public VkPipelineVertexInputStateCreateInfo*   PVertexInputState;
    public VkPipelineInputAssemblyStateCreateInfo* PInputAssemblyState;
    public VkPipelineTessellationStateCreateInfo*  PTessellationState;
    public VkPipelineViewportStateCreateInfo*      PViewportState;
    public VkPipelineRasterizationStateCreateInfo* PRasterizationState;
    public VkPipelineMultisampleStateCreateInfo*   PMultisampleState;
    public VkPipelineDepthStencilStateCreateInfo*  PDepthStencilState;
    public VkPipelineColorBlendStateCreateInfo*    PColorBlendState;
    public VkPipelineDynamicStateCreateInfo*       PDynamicState;
    public VkHandle<VkPipelineLayout>              Layout;
    public VkHandle<VkRenderPass>                  RenderPass;
    public uint                                    Subpass;
    public VkHandle<VkPipeline>                    BasePipelineHandle;
    public int                                     BasePipelineIndex;

    public VkGraphicsPipelineCreateInfo() =>
        this.SType = VkStructureType.GraphicsPipelineCreateInfo;
}
