
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Extensions;

namespace ThirdParty.Vulkan;

#pragma warning disable IDE0001

public unsafe class VkSurfaceKHR : DisposableManagedHandle<VkSurfaceKHR>
{
    private readonly VkSurfaceExtensionKHR extension;

    internal VkSurfaceKHR(VkHandle<VkSurfaceKHR> handle, VkSurfaceExtensionKHR extension) : base(handle) =>
        this.extension = extension;

    protected override void Disposed() =>
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
    public bool GetSupport(VkPhysicalDevice physicalDevice, uint queueFamilyIndex) =>
        this.extension.GetPhysicalDeviceSurfaceSupport(physicalDevice, queueFamilyIndex, this);
}
