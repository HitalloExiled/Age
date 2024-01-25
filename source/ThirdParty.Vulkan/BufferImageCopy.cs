using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkBufferImageCopy" />
public unsafe record BufferImageCopy : NativeReference<VkBufferImageCopy>
{
    private Extent3D?               imageExtent;
    private Offset3D?               imageOffset;
    private ImageSubresourceLayers? imageSubresource;

    public ulong BufferOffset
    {
        get => this.PNative->bufferOffset;
        init => this.PNative->bufferOffset = value;
    }

    public uint BufferRowLength
    {
        get => this.PNative->bufferRowLength;
        init => this.PNative->bufferRowLength = value;
    }

    public uint BufferImageHeight
    {
        get => this.PNative->bufferImageHeight;
        init => this.PNative->bufferImageHeight = value;
    }

    public ImageSubresourceLayers? ImageSubresource
    {
        get => this.imageSubresource;
        init => this.PNative->imageSubresource = this.imageSubresource = value;
    }

    public Offset3D? ImageOffset
    {
        get => this.imageOffset;
        init => this.PNative->imageOffset = this.imageOffset = value;
    }

    public Extent3D? ImageExtent
    {
        get => this.imageExtent;
        init => this.PNative->imageExtent = this.imageExtent = value;
    }
}
