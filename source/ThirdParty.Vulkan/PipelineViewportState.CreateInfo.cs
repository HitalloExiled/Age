using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static class PipelineViewportState
{
    /// <inheritdoc cref="VkPipelineViewportStateCreateInfo" />
    public unsafe record CreateInfo : NativeReference<VkPipelineViewportStateCreateInfo>
    {
        private Viewport[] viewports = [];
        private Rect2D[]   scissors  = [];

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public uint Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public uint ViewportCount
        {
            get => this.PNative->viewportCount;
            init => this.PNative->viewportCount = value;
        }

        public Viewport[] Viewports
        {
            get => this.viewports;
            init => Init(ref this.viewports, ref this.PNative->pViewports, ref this.PNative->viewportCount, value);
        }

        public uint ScissorCount
        {
            get => this.PNative->scissorCount;
            init => this.PNative->scissorCount = value;
        }

        public Rect2D[] Scissors
        {
            get => this.scissors;
            init => Init(ref this.scissors, ref this.PNative->pScissors, ref this.PNative->scissorCount, value);
        }

        protected override void OnFinalize()
        {
            Free(ref this.PNative->pViewports);
            Free(ref this.PNative->pScissors);
        }
    }
}
