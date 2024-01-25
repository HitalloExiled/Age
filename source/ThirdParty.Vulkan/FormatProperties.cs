using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe record FormatProperties : NativeReference<VkFormatProperties>
{
    public FormatFeatureFlags LinearTilingFeatures  => this.PNative->linearTilingFeatures;
    public FormatFeatureFlags OptimalTilingFeatures => this.PNative->optimalTilingFeatures;
    public FormatFeatureFlags BufferFeatures        => this.PNative->bufferFeatures;

    internal FormatProperties(in VkFormatProperties value) : base(value) { }
}
