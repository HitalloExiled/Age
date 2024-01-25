namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkRenderPassCreateInfo.html">VkRenderPassCreateInfo</see>
/// </summary>
public unsafe struct VkRenderPassCreateInfo
{
    public readonly VkStructureType sType;

    public void*                    pNext;
    public VkRenderPassCreateFlags  flags;
    public uint                     attachmentCount;
    public VkAttachmentDescription* pAttachments;
    public uint                     subpassCount;
    public VkSubpassDescription*    pSubpasses;
    public uint                     dependencyCount;
    public VkSubpassDependency*     pDependencies;

    public VkRenderPassCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO;
}
