using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkSubpassDescription" />
public unsafe record SubpassDescription : NativeReference<VkSubpassDescription>
{
    private AttachmentReference[] inputAttachments    = [];
    private AttachmentReference[] colorAttachments    = [];
    private AttachmentReference[] resolveAttachments  = [];
    private AttachmentReference?  depthStencilAttachment;
    private uint[]                preserveAttachments = [];

    public VkSubpassDescriptionFlags Flags
    {
        get => this.PNative->flags;
        init => this.PNative->flags = value;
    }

    public VkPipelineBindPoint PipelineBindPoint
    {
        get => this.PNative->pipelineBindPoint;
        init => this.PNative->pipelineBindPoint = value;
    }

    public AttachmentReference[] InputAttachments
    {
        get => this.inputAttachments;
        init => Init(ref this.inputAttachments, ref this.PNative->pInputAttachments, ref this.PNative->inputAttachmentCount, value);
    }

    public AttachmentReference[] ColorAttachments
    {
        get => this.colorAttachments;
        init => Init(ref this.colorAttachments, ref this.PNative->pColorAttachments, ref this.PNative->colorAttachmentCount, value);
    }

    public AttachmentReference[] ResolveAttachments
    {
        get => this.resolveAttachments;
        init => Init(ref this.resolveAttachments, ref this.PNative->pResolveAttachments, ref this.PNative->colorAttachmentCount, value);
    }

    public AttachmentReference? DepthStencilAttachment
    {
        get => this.depthStencilAttachment;
        init => this.PNative->pDepthStencilAttachment = this.depthStencilAttachment = value;
    }

    public uint[] PreserveAttachments
    {
        get => this.preserveAttachments;
        init => Init(ref this.preserveAttachments, ref this.PNative->pPreserveAttachments, ref this.PNative->preserveAttachmentCount, value);
    }

    protected override void OnFinalize()
    {
        Free(ref this.PNative->pInputAttachments);
        Free(ref this.PNative->pColorAttachments);
        Free(ref this.PNative->pResolveAttachments);
        Free(ref this.PNative->pPreserveAttachments);
    }
}
