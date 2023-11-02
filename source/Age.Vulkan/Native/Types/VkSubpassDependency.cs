using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying a subpass dependency.</para>
/// <para>If srcSubpass is equal to dstSubpass then the <see cref="VkSubpassDependency"/> does not directly define a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies">dependency</see>. Instead, it enables pipeline barriers to be used in a render pass instance within the identified subpass, where the scopes of one pipeline barrier must be a subset of those described by one subpass dependency. Subpass dependencies specified in this way that <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-framebuffer-regions">include framebuffer-space</see> stages in the srcStageMask must only include framebuffer-space stages in dstStageMask, and must include <see cref="VK_DEPENDENCY_BY_REGION_BIT"/>. When a subpass dependency is specified in this way for a subpass that has more than one view in its view mask, its dependencyFlags must include <see cref="VK_DEPENDENCY_VIEW_LOCAL_BIT"/>.</para>
/// <para>If srcSubpass and dstSubpass are not equal, when a render pass instance which includes a subpass dependency is submitted to a queue, it defines a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies">dependency</see> between the subpasses identified by srcSubpass and dstSubpass.</para>
/// <para>If srcSubpass is equal to <see cref="Vk.VK_SUBPASS_EXTERNAL"/>, the first <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-scopes">synchronization scope</see> includes commands that occur earlier in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-submission-order">submission order</see> than the <see cref="Vk.CmdBeginRenderPass"/> used to begin the render pass instance. Otherwise, the first set of commands includes all commands submitted as part of the subpass instance identified by srcSubpass and any <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#renderpass-load-operations">load</see>, <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#renderpass-store-operations">store</see>, or <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#renderpass-resolve-operations">multisample resolve</see> operations on attachments used in srcSubpass. In either case, the first synchronization scope is limited to operations on the pipeline stages determined by the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-pipeline-stages-masks">source stage mask</see> specified by srcStageMask.</para>
/// <para>If dstSubpass is equal to <see cref="Vk.VK_SUBPASS_EXTERNAL"/>, the second <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-scopes">synchronization scope</see> includes commands that occur later in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-submission-order">submission order</see> than the <see cref="Vk.CmdEndRenderPass"/> used to end the render pass instance. Otherwise, the second set of commands includes all commands submitted as part of the subpass instance identified by dstSubpass and any <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#renderpass-load-operations">load</see>, <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#renderpass-store-operations">store</see>, and <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#renderpass-resolve-operations">multisample resolve</see> operations on attachments used in dstSubpass. In either case, the second synchronization scope is limited to operations on the pipeline stages determined by the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-pipeline-stages-masks">destination stage mask</see> specified by dstStageMask.</para>
/// <para>The first <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-access-scopes">access scope</see> is limited to accesses in the pipeline stages determined by the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-pipeline-stages-masks">source stage mask</see> specified by srcStageMask. It is also limited to access types in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-access-masks">source access mask</see> specified by srcAccessMask.</para>
/// <para>The second <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-access-scopes">access scope</see> is limited to accesses in the pipeline stages determined by the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-pipeline-stages-masks">destination stage mask</see> specified by dstStageMask. It is also limited to access types in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-access-masks">destination access mask</see> specified by dstAccessMask.</para>
/// <para>The <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-available-and-visible">availability and visibility operations</see> defined by a subpass dependency affect the execution of <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#renderpass-layout-transitions">image layout transitions</see> within the render pass.</para>
/// <remarks>
/// <para>Note: For non-attachment resources, the memory dependency expressed by subpass dependency is nearly identical to that of a <see cref="VkMemoryBarrier"/> (with matching srcAccessMask and dstAccessMask parameters) submitted as a part of a <see cref="Vk.CmdPipelineBarrier"/> (with matching srcStageMask and dstStageMask parameters). The only difference being that its scopes are limited to the identified subpasses rather than potentially affecting everything before and after.</para>
/// <para>For attachments however, subpass dependencies work more like a <see cref="VkImageMemoryBarrier"/> defined similarly to the <see cref="VkMemoryBarrier"/> above, the queue family indices set to <see cref="Vk.VK_QUEUE_FAMILY_IGNORED"/>, and layouts as follows:</para>
/// <list type="bullet">
/// <item>The equivalent to oldLayout is the attachment’s layout according to the subpass description for srcSubpass.</item>
/// <item>The equivalent to newLayout is the attachment’s layout according to the subpass description for dstSubpass.</item>
/// </list>
/// </remarks>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkSubpassDependency
{
    /// <summary>
    /// The subpass index of the first subpass in the dependency, or <see cref="Vk.VK_SUBPASS_EXTERNAL"/>.
    /// </summary>
    public uint srcSubpass;

    /// <summary>
    /// The subpass index of the second subpass in the dependency, or <see cref="Vk.VK_SUBPASS_EXTERNAL"/>.
    /// </summary>
    public uint dstSubpass;

    /// <summary>
    /// A bitmask of <see cref="VkPipelineStageFlagBits"/> specifying the source stage mask.
    /// </summary>
    public VkPipelineStageFlags srcStageMask;

    /// <summary>
    /// A bitmask of <see cref="VkPipelineStageFlagBits"/> specifying the destination stage mask
    /// </summary>
    public VkPipelineStageFlags dstStageMask;

    /// <summary>
    /// A bitmask of <see cref="VkAccessFlagBits"/> specifying a source access mask.
    /// </summary>
    public VkAccessFlags srcAccessMask;

    /// <summary>
    /// A bitmask of <see cref="VkAccessFlagBits"/> specifying a destination access mask.
    /// </summary>
    public VkAccessFlags dstAccessMask;

    /// <summary>
    /// A bitmask of <see cref="VkDependencyFlagBits"/>.
    /// </summary>
    public VkDependencyFlags dependencyFlags;
}
