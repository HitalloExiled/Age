using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class RenderPass
{
    /// <inheritdoc cref="VkRenderPassCreateInfo" />
    public unsafe record CreateInfo : NativeReference<VkRenderPassCreateInfo>
    {
        private AttachmentDescription[] attachments  = [];
        private VkSubpassDescription[]  subpasses    = [];
        private VkSubpassDependency[]   dependencies = [];

        public nint PNext
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public RenderPassCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public AttachmentDescription[] Attachments
        {
            get => this.attachments;
            init => Init(ref this.attachments, ref this.PNative->pAttachments, ref this.PNative->attachmentCount, value);
        }

        public VkSubpassDescription[] Subpasses
        {
            get => this.subpasses;
            init => Init(ref this.subpasses, ref this.PNative->pSubpasses, ref this.PNative->subpassCount, value);
        }

        public VkSubpassDependency[] Dependencies
        {
            get => this.dependencies;
            init => Init(ref this.dependencies, ref this.PNative->pDependencies, ref this.PNative->dependencyCount, value);
        }

        protected override void OnFinalize()
        {
            Free(ref this.PNative->pAttachments);
            Free(ref this.PNative->pSubpasses);
            Free(ref this.PNative->pDependencies);
        }
    }
}
