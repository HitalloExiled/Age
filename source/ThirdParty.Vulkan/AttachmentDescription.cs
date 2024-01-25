using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkAttachmentDescription" />
public unsafe record AttachmentDescription : NativeReference<VkAttachmentDescription>
{
    public VkAttachmentDescriptionFlags Flags
    {
        get => this.PNative->flags;
        init => this.PNative->flags = value;
    }

    public VkFormat Format
    {
        get => this.PNative->format;
        init => this.PNative->format = value;
    }

    public VkSampleCountFlagBits Samples
    {
        get => this.PNative->samples;
        init => this.PNative->samples = value;
    }

    public VkAttachmentLoadOp LoadOp
    {
        get => this.PNative->loadOp;
        init => this.PNative->loadOp = value;
    }

    public VkAttachmentStoreOp StoreOp
    {
        get => this.PNative->storeOp;
        init => this.PNative->storeOp = value;
    }

    public VkAttachmentLoadOp StencilLoadOp
    {
        get => this.PNative->stencilLoadOp;
        init => this.PNative->stencilLoadOp = value;
    }

    public VkAttachmentStoreOp StencilStoreOp
    {
        get => this.PNative->stencilStoreOp;
        init => this.PNative->stencilStoreOp = value;
    }

    public VkImageLayout InitialLayout
    {
        get => this.PNative->initialLayout;
        init => this.PNative->initialLayout = value;
    }

    public VkImageLayout FinalLayout
    {
        get => this.PNative->finalLayout;
        init => this.PNative->finalLayout = value;
    }

}
