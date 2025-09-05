using Age.Rendering.Resources;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Vulkan;

public ref struct FramebufferCreateInfo
{
    public struct Attachment(Image image, VkImageAspectFlags imageAspect)
    {
        public Image              Image       = image;
        public VkImageAspectFlags ImageAspect = imageAspect;
    }

    public required RenderPass               RenderPass;
    public required ReadOnlySpan<Attachment> Attachments;
}
