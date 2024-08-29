using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineViewportStateCreateInfo.html">VkPipelineViewportStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineViewportStateCreateInfo
{
    public readonly VkStructureType SType;

    public void*                              PNext;
    public VkPipelineViewportStateCreateFlags Flags;
    public uint                               ViewportCount;
    public VkViewport*                        PViewports;
    public uint                               ScissorCount;
    public VkRect2D*                          PScissors;

    public VkPipelineViewportStateCreateInfo() =>
        this.SType = VkStructureType.PipelineViewportStateCreateInfo;
}
