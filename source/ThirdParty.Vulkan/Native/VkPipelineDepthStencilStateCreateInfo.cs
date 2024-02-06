using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineDepthStencilStateCreateInfo.html">VkPipelineDepthStencilStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineDepthStencilStateCreateInfo
{
    public readonly VkStructureType SType;

    public void*                                  PNext;
    public VkPipelineDepthStencilStateCreateFlags Flags;
    public VkBool32                               DepthTestEnable;
    public VkBool32                               DepthWriteEnable;
    public VkCompareOp                            DepthCompareOp;
    public VkBool32                               DepthBoundsTestEnable;
    public VkBool32                               StencilTestEnable;
    public VkStencilOpState                       Front;
    public VkStencilOpState                       Back;
    public float                                  MinDepthBounds;
    public float                                  MaxDepthBounds;

    public VkPipelineDepthStencilStateCreateInfo() =>
        this.SType = VkStructureType.PipelineDepthStencilStateCreateInfo;
}
