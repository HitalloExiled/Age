using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static class PipelineViewportState
{
    /// <summary>
    /// Structure specifying parameters of a newly created pipeline viewport state.
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkPipelineViewportStateCreateInfo>
    {
        private Rect2D[]   scissors  = [];
        private Viewport[] viewports = [];

        /// <summary>
        /// Structure extending this structure.
        /// </summary>
        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        public uint Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        /// <summary>
        /// The number of viewports used by the pipeline.
        /// </summary>
        public uint ViewportCount
        {
            get => this.PNative->viewportCount;
            init => this.PNative->viewportCount = value;
        }

        /// <summary>
        /// An array of <see cref="Viewport"/> structures, defining the viewport transforms. If the viewport state is dynamic, this member is ignored.
        /// </summary>
        public Viewport[] Viewports
        {
            get => this.viewports;
            init => Init(ref this.viewports, ref this.PNative->pViewports, ref this.PNative->viewportCount, value);
        }

        /// <summary>
        /// The number of scissors and must match the number of viewports.
        /// </summary>
        public uint ScissorCount
        {
            get => this.PNative->scissorCount;
            init => this.PNative->scissorCount = value;
        }

        /// <summary>
        /// An array of <see cref="Rect2D"/> structures defining the rectangular bounds of the scissor for the corresponding viewport. If the scissor state is dynamic, this member is ignored.
        /// </summary>
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
