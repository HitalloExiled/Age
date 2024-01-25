using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class RenderPass
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkRenderPassBeginInfo.html">VkRenderPassBeginInfo</see>
    /// </summary>
    public unsafe record BeginInfo : NativeReference<VkRenderPassBeginInfo>
    {
        private ClearValue[] clearValues = [];
        private Framebuffer? framebuffer;
        private Rect2D?      renderArea;
        private RenderPass?  renderPass;

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public RenderPass? RenderPass
        {
            get => this.renderPass;
            init => this.PNative->renderPass = this.renderPass = value;
        }

        public Framebuffer? Framebuffer
        {
            get => this.framebuffer;
            init => this.PNative->framebuffer = this.framebuffer = value;
        }

        public Rect2D? RenderArea
        {
            get => this.renderArea;
            init => this.PNative->renderArea = this.renderArea = value;
        }

        public ClearValue[] ClearValues
        {
            get => this.clearValues;
            init => Init(ref this.clearValues, ref this.PNative->pClearValues, ref this.PNative->clearValueCount, value);
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pClearValues);
    }
}
