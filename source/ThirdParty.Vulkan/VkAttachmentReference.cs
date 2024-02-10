using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkAttachmentReference.html">VkAttachmentReference</see>
/// </summary>
public struct VkAttachmentReference
{
    public uint          Attachment;
    public VkImageLayout Layout;
}
