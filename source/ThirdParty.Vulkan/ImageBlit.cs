using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkImageBlit" />
public unsafe record ImageBlit : NativeReference<VkImageBlit>
{
    private readonly Offset3D[] dstOffsets = new Offset3D[2];
    private readonly Offset3D[] srcOffsets = new Offset3D[2];

    private ImageSubresourceLayers? dstSubresource;
    private ImageSubresourceLayers? srcSubresource;

    public ImageSubresourceLayers? SrcSubresource
    {
        get => this.srcSubresource;
        init => this.PNative->srcSubresource = this.srcSubresource = value;
    }

    public Offset3D[] SrcOffsets
    {
        get => this.srcOffsets;
        init => Init(this.srcOffsets, (VkOffset3D*)this.PNative->srcOffsets, 2, value, nameof(this.DstOffsets));
    }

    public ImageSubresourceLayers? DstSubresource
    {
        get => this.dstSubresource;
        init => this.PNative->dstSubresource = this.dstSubresource = value;
    }

    public Offset3D[] DstOffsets
    {
        get => this.dstOffsets;
        init => Init(this.dstOffsets, (VkOffset3D*)this.PNative->dstOffsets, 2, value, nameof(this.DstOffsets));
    }
}
