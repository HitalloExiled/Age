
using ThirdParty.Vulkan.Enums.KHR;
using ThirdParty.Vulkan.Extensions.KHR;

namespace ThirdParty.Vulkan.KHR;

#pragma warning disable IDE0001

public unsafe class Surface : DisposableNativeHandle
{
    private readonly SurfaceExtension extension;

    internal Surface(VkSurfaceKHR handle, SurfaceExtension extension) : base(handle) =>
        this.extension = extension;

    protected override void OnDispose() =>
        this.extension.DestroySurface(this);

    /// <inheritdoc cref="SurfaceExtension.GetPhysicalDeviceSurfaceCapabilities" />
    public SurfaceCapabilities GetCapabilities(PhysicalDevice physicalDevice) =>
        this.extension.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, this);

    /// <inheritdoc cref="SurfaceExtension.GetPhysicalDeviceSurfaceFormats" />
    public SurfaceFormat[] GetFormats(PhysicalDevice physicalDevice) =>
        this.extension.GetPhysicalDeviceSurfaceFormats(physicalDevice, this);

    /// <inheritdoc cref="SurfaceExtension.GetPhysicalDeviceSurfacePresentModes" />
    public PresentMode[] GetPresentModes(PhysicalDevice physicalDevice) =>
        this.extension.GetPhysicalDeviceSurfacePresentModes(physicalDevice, this);
}
