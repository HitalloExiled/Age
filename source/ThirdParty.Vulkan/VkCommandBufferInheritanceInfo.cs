using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandBufferInheritanceInfo.html">VkCommandBufferInheritanceInfo</see>
/// </summary>
public unsafe struct VkCommandBufferInheritanceInfo
{
    public readonly VkStructureType SType;

    public void*                         PNext;
    public VkHandle<VkRenderPass>        RenderPass;
    public uint                          Subpass;
    public VkHandle<VkFramebuffer>       Framebuffer;
    public VkBool32                      OcclusionQueryEnable;
    public VkQueryControlFlags           QueryFlags;
    public VkQueryPipelineStatisticFlags PipelineStatistics;

    public VkCommandBufferInheritanceInfo() =>
        this.SType = VkStructureType.CommandBufferInheritanceInfo;
}
