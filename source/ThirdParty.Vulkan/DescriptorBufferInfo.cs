using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorBufferInfo.html">VkDescriptorBufferInfo</see>
/// </summary>
public unsafe record DescriptorBufferInfo : NativeReference<VkDescriptorBufferInfo>
{
    private Buffer? buffer;

    public Buffer? Buffer
    {
        get => this.buffer;
        init => this.PNative->buffer = this.buffer = value;
    }

    public ulong Offset
    {
        get => this.PNative->offset;
        init => this.PNative->offset = value;
    }

    public ulong Range
    {
        get => this.PNative->range;
        init => this.PNative->range = value;
    }
}
