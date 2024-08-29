using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineRasterizationStateCreateInfo.html">VkPipelineRasterizationStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineRasterizationStateCreateInfo
{
    public readonly VkStructureType SType;

    public void*                                   PNext;
    public VkPipelineRasterizationStateCreateFlags Flags;
    public VkBool32                                DepthClampEnable;
    public VkBool32                                RasterizerDiscardEnable;
    public VkPolygonMode                           PolygonMode;
    public VkCullModeFlags                         CullMode;
    public VkFrontFace                             FrontFace;
    public VkBool32                                DepthBiasEnable;
    public float                                   DepthBiasConstantFactor;
    public float                                   DepthBiasClamp;
    public float                                   DepthBiasSlopeFactor;
    public float                                   LineWidth;

    public VkPipelineRasterizationStateCreateInfo() =>
        this.SType = VkStructureType.PipelineRasterizationStateCreateInfo;
}
