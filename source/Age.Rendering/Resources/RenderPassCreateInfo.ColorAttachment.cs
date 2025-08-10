using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public readonly partial struct RenderPassCreateInfo
{
    public readonly struct ColorAttachment
    {
        public VkAttachmentDescription  Color   { get; init; }
        public VkAttachmentDescription? Resolve { get; init; }
    };
}
