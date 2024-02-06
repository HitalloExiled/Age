
using ThirdParty.Vulkan.Enums.KHR;
using ThirdParty.Vulkan.Extensions.KHR;
using ThirdParty.Vulkan.Native.KHR;

namespace ThirdParty.Vulkan.KHR;

#pragma warning disable IDE0001

public unsafe class VkSurfaceKHR : DisposableManagedHandle<VkSurfaceKHR>
{
    private readonly VkSurfaceExtensionKHR extension;

    internal VkSurfaceKHR(VkHandle<VkSurfaceKHR> handle, VkSurfaceExtensionKHR extension) : base(handle) =>
        this.extension = extension;

    protected override void OnDispose() =>
        this.extension.DestroySurface(this);

    /// <inheritdoc cref="VkSurfaceExtensionKHR.GetPhysicalDeviceSurfaceCapabilities" />
    public void GetCapabilities(VkPhysicalDevice physicalDevice, out VkSurfaceCapabilitiesKHR surfaceCapabilities) =>
        this.extension.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, this, out surfaceCapabilities);

    /// <inheritdoc cref="VkSurfaceExtensionKHR.GetPhysicalDeviceSurfaceFormats" />
    public VkSurfaceFormatKHR[] GetFormats(VkPhysicalDevice physicalDevice) =>
        this.extension.GetPhysicalDeviceSurfaceFormats(physicalDevice, this);

    /// <inheritdoc cref="VkSurfaceExtensionKHR.GetPhysicalDeviceSurfacePresentModes" />
    public VkPresentModeKHR[] GetPresentModes(VkPhysicalDevice physicalDevice) =>
        this.extension.GetPhysicalDeviceSurfacePresentModes(physicalDevice, this);
}
