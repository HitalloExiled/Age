using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static class PipelineColorBlendState
{
    /// <summary>
    /// Structure specifying parameters of a newly created pipeline color blend state.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public unsafe record CreateInfo : NativeReference<VkPipelineColorBlendStateCreateInfo>
    {
        private readonly float[] blendConstants = new float[4];

        private PipelineColorBlendAttachmentState[] attachments = [];

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public PipelineColorBlendStateCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public bool LogicOpEnable
        {
            get => this.PNative->logicOpEnable;
            init => this.PNative->logicOpEnable = value;
        }

        public LogicOp LogicOp
        {
            get => this.PNative->logicOp;
            init => this.PNative->logicOp = value;
        }

        public PipelineColorBlendAttachmentState[] Attachments
        {
            get => this.attachments;
            init => Init(ref this.attachments, ref this.PNative->pAttachments, ref this.PNative->attachmentCount, value);
        }

        public float[] BlendConstants
        {
            get => this.blendConstants;
            init => Init(this.blendConstants, this.PNative->blendConstants, 4, value, nameof(this.BlendConstants));
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pAttachments);
    }
}
