using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkRenderPassCreateInfo.html">VkRenderPassCreateInfo</see>
/// </summary>
public unsafe struct VkRenderPassCreateInfo
{
    public readonly VkStructureType SType;

    public void*                    PNext;
    public VkRenderPassCreateFlags  Flags;
    public uint                     AttachmentCount;
    public VkAttachmentDescription* PAttachments;
    public uint                     SubpassCount;
    public VkSubpassDescription*    PSubpasses;
    public uint                     DependencyCount;
    public VkSubpassDependency*     PDependencies;

    public VkRenderPassCreateInfo() =>
        this.SType = VkStructureType.RenderPassCreateInfo;
}
