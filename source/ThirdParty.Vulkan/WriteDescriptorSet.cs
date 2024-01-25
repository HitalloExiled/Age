using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkWriteDescriptorSet" />
public unsafe record WriteDescriptorSet : NativeReference<VkWriteDescriptorSet>
{
    private DescriptorSet?         dstSet;
    private DescriptorImageInfo[]  imageInfo = [];
    private DescriptorBufferInfo[] bufferInfo = [];
    private BufferView[]           texelBufferView = [];

    public nint Next
    {
        get => (nint)this.PNative->pNext;
        init => this.PNative->pNext = value.ToPointer();
    }

    public DescriptorSet? DstSet
    {
        get => this.dstSet;
        init => this.PNative->dstSet = this.dstSet = value;
    }

    public uint DstBinding
    {
        get => this.PNative->dstBinding;
        init => this.PNative->dstBinding = value;
    }

    public uint DstArrayElement
    {
        get => this.PNative->dstArrayElement;
        init => this.PNative->dstArrayElement = value;
    }

    public uint DescriptorCount
    {
        get => this.PNative->descriptorCount;
        init => this.PNative->descriptorCount = value;
    }

    public DescriptorType DescriptorType
    {
        get => this.PNative->descriptorType;
        init => this.PNative->descriptorType = value;
    }

    public DescriptorImageInfo[] ImageInfo
    {
        get => this.imageInfo;
        init => Init(ref this.imageInfo, ref this.PNative->pImageInfo, value);
    }

    public DescriptorBufferInfo[] BufferInfo
    {
        get => this.bufferInfo;
        init => Init(ref this.bufferInfo, ref this.PNative->pBufferInfo, value);
    }

    public BufferView[] TexelBufferView
    {
        get => this.texelBufferView;
        init => Init(ref this.texelBufferView, ref this.PNative->pTexelBufferView, value);
    }

    protected override void OnFinalize()
    {
        Free(ref this.PNative->pImageInfo);
        Free(ref this.PNative->pBufferInfo);
        Free(ref this.PNative->pTexelBufferView);
    }
}
