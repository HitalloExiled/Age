using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineMultisampleStateCreateInfo.html">VkPipelineMultisampleStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineMultisampleStateCreateInfo
{
    public readonly VkStructureType sType;

    public void*                                 PNext;
    public VkPipelineMultisampleStateCreateFlags Flags;
    public VkSampleCountFlags                    RasterizationSamples;
    public VkBool32                              SampleShadingEnable;
    public float                                 MinSampleShading;
    public VkHandle<VkSampleMask>*               PSampleMask;
    public VkBool32                              AlphaToCoverageEnable;
    public VkBool32                              AlphaToOneEnable;

    public VkPipelineMultisampleStateCreateInfo() =>
        this.sType = VkStructureType.PipelineMultisampleStateCreateInfo;
}
