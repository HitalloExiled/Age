using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSubpassDependency.html">VkSubpassDependency</see>
/// </summary>
public struct VkSubpassDependency
{
    public uint                 SrcSubpass;
    public uint                 DstSubpass;
    public VkPipelineStageFlags SrcStageMask;
    public VkPipelineStageFlags DstStageMask;
    public VkAccessFlags        SrcAccessMask;
    public VkAccessFlags        DstAccessMask;
    public VkDependencyFlags    DependencyFlags;
}
