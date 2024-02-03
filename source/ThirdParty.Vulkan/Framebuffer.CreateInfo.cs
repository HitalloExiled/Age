using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class Framebuffer
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFramebufferCreateInfo.html">VkFramebufferCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkFramebufferCreateInfo>
    {
        private RenderPass? renderPass;
        private ImageView[] attachments = [];

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public FramebufferCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public RenderPass? RenderPass
        {
            get => this.renderPass;
            init => this.PNative->renderPass = this.renderPass = value;
        }

        public ImageView[] Attachments
        {
            get => this.attachments;
            init => Init(ref this.attachments, ref this.PNative->pAttachments, ref this.PNative->attachmentCount, value);
        }

        public uint Width
        {
            get => this.PNative->width;
            init => this.PNative->width = value;
        }

        public uint Height
        {
            get => this.PNative->height;
            init => this.PNative->height = value;
        }

        public uint Layers
        {
            get => this.PNative->layers;
            init => this.PNative->layers = value;
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pAttachments);
    }
}
