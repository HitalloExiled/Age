namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSubpassDependency.html">VkSubpassDependency</see>
/// </summary>
public struct VkSubpassDependency
{
    public uint                 srcSubpass;
    public uint                 dstSubpass;
    public VkPipelineStageFlags srcStageMask;
    public VkPipelineStageFlags dstStageMask;
    public VkAccessFlags        srcAccessMask;
    public VkAccessFlags        dstAccessMask;
    public VkDependencyFlags    dependencyFlags;
}
