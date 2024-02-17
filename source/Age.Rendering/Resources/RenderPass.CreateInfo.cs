using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public partial record RenderPass
{
    public partial struct CreateInfo
    {
        public struct ColorAttachment
        {
            public VkImageLayout           Layout;
            public VkAttachmentDescription Color;
            public VkAttachmentDescription Resolve;
        };

        public ColorAttachment[] ColorAttachments = [];

        public VkAttachmentReference DepthStencilAttachment;

        public CreateInfo() { }
    }
}
