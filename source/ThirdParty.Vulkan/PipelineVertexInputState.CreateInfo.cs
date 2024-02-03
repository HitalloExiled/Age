using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static class PipelineVertexInputState
{
    /// <inheritdoc cref="VkPipelineVertexInputStateCreateInfo" />
    public unsafe record CreateInfo : NativeReference<VkPipelineVertexInputStateCreateInfo>
    {
        private VertexInputBindingDescription[]   vertexBindingDescriptions   = [];
        private VertexInputAttributeDescription[] vertexAttributeDescriptions = [];

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

        public VertexInputBindingDescription[] VertexBindingDescriptions
        {
            get => this.vertexBindingDescriptions;
            init => Init(ref this.vertexBindingDescriptions, ref this.PNative->pVertexBindingDescriptions, ref this.PNative->vertexBindingDescriptionCount, value);
        }

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
