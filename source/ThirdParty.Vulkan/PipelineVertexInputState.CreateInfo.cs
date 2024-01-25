using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static class PipelineVertexInputState
{
    /// <summary>
    /// Structure specifying parameters of a newly created pipeline vertex input state.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public unsafe record CreateInfo : NativeReference<VkPipelineVertexInputStateCreateInfo>
    {
        private VertexInputAttributeDescription[] vertexAttributeDescriptions = [];
        private VertexInputBindingDescription[]   vertexBindingDescriptions   = [];

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
        /// A pointer to an array of VkVertexInputBindingDescription structures.
        /// </summary>
        public VertexInputBindingDescription[] VertexBindingDescriptions
        {
            get => this.vertexBindingDescriptions;
            init => Init(ref this.vertexBindingDescriptions, ref this.PNative->pVertexBindingDescriptions, ref this.PNative->vertexBindingDescriptionCount, value);
        }

        /// <summary>
        /// A pointer to an array of VkVertexInputAttributeDescription structures.
        /// </summary>
        public VertexInputAttributeDescription[] VertexAttributeDescriptions
        {
            get => this.vertexAttributeDescriptions;
            init => Init(ref this.vertexAttributeDescriptions, ref this.PNative->pVertexAttributeDescriptions, ref this.PNative->vertexAttributeDescriptionCount, value);
        }

        protected override void OnFinalize()
        {
            Free(ref this.PNative->pVertexBindingDescriptions);
            Free(ref this.PNative->pVertexAttributeDescriptions);
        }
    }
}
