using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkAttachmentDescription.html">VkAttachmentDescription</see>
/// </summary>
public struct VkAttachmentDescription
{
    public VkAttachmentDescriptionFlags Flags;
    public VkFormat                     Format;
    public VkSampleCountFlags           Samples;
    public VkAttachmentLoadOp           LoadOp;
    public VkAttachmentStoreOp          StoreOp;
    public VkAttachmentLoadOp           StencilLoadOp;
    public VkAttachmentStoreOp          StencilStoreOp;
    public VkImageLayout                InitialLayout;
    public VkImageLayout                FinalLayout;
}
