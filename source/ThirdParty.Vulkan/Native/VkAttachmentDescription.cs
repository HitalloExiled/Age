namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkAttachmentDescription.html">VkAttachmentDescription</see>
/// </summary>
public struct VkAttachmentDescription
{
    public VkAttachmentDescriptionFlags flags;
    public VkFormat                     format;
    public VkSampleCountFlagBits        samples;
    public VkAttachmentLoadOp           loadOp;
    public VkAttachmentStoreOp          storeOp;
    public VkAttachmentLoadOp           stencilLoadOp;
    public VkAttachmentStoreOp          stencilStoreOp;
    public VkImageLayout                initialLayout;
    public VkImageLayout                finalLayout;
}
