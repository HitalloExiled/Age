using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Flags.KHR;
using ThirdParty.Vulkan.Native.KHR;

namespace ThirdParty.Vulkan.KHR;

/// <inheritdoc cref="VkSurfaceCapabilitiesKHR" />
public unsafe record SurfaceCapabilities : NativeReference<VkSurfaceCapabilitiesKHR>
{
    private Extent2D? currentExtent;
    private Extent2D? minImageExtent;
    private Extent2D? maxImageExtent;

    public SurfaceTransformFlags CurrentTransform        => this.PNative->currentTransform;
    public Extent2D              CurrentExtent           => this.currentExtent ??= new(this.PNative->currentExtent);
    public uint                  MinImageCount           => this.PNative->minImageCount;
    public Extent2D              MinImageExtent          => this.minImageExtent ??= new(this.PNative->minImageExtent);
    public uint                  MaxImageArrayLayers     => this.PNative->maxImageArrayLayers;
    public uint                  MaxImageCount           => this.PNative->maxImageCount;
    public Extent2D              MaxImageExtent          => this.maxImageExtent ??= new(this.PNative->maxImageExtent);
    public SurfaceTransformFlags SupportedTransforms     => this.PNative->supportedTransforms;
    public CompositeAlphaFlags   SupportedCompositeAlpha => this.PNative->supportedCompositeAlpha;
    public ImageUsageFlags       SupportedUsageFlags     => this.PNative->supportedUsageFlags;

    internal SurfaceCapabilities(VkSurfaceCapabilitiesKHR value) : base(value) { }
}
