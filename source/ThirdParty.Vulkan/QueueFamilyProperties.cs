using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkQueueFamilyProperties" />
public unsafe record QueueFamilyProperties : NativeReference<VkQueueFamilyProperties>
{
    private Extent3D? minImageTransferGranularity;

    public QueueFlags QueueFlags                  => this.PNative->queueFlags;
    public uint       QueueCount                  => this.PNative->queueCount;
    public uint       TimestampValidBits          => this.PNative->timestampValidBits;
    public Extent3D   MinImageTransferGranularity => this.minImageTransferGranularity ??= new(this.PNative->minImageTransferGranularity);

    internal QueueFamilyProperties(in VkQueueFamilyProperties value) : base(value) { }
}
