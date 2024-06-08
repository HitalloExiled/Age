using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Vulkan;

public ref struct FramebufferCreateInfo
{
    public struct Attachment
    {
        public required Image              Image;
        public required VkFormat           Format;
        public required VkImageAspectFlags ImageAspect;
    }

    public required RenderPass       RenderPass;
    public required Span<Attachment> Attachments;
}
