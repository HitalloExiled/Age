using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkAttachmentReference" />
public unsafe record AttachmentReference : NativeReference<VkAttachmentReference>
{
    public uint Attachment
    {
        get => this.PNative->attachment;
        init => this.PNative->attachment = value;
    }

    public VkImageLayout Layout
    {
        get => this.PNative->layout;
        init => this.PNative->layout = value;
    }
}
