using ThirdParty.Vulkan.Extensions.KHR;

namespace ThirdParty.Vulkan.KHR;

public partial class VkSwapchainKHR : DisposableManagedHandle<VkSwapchainKHR>
{
    private readonly VkSwapchainExtensionKHR extension;

    internal VkSwapchainKHR(VkHandle<VkSwapchainKHR> handle, VkSwapchainExtensionKHR extension) : base(handle) =>
        this.extension = extension;

    protected override void OnDispose() =>
        this.extension.DestroySwapchain(this);

    /// <inheritdoc cref="VkSwapchainExtensionKHR.AcquireNextImage" />
    public uint AcquireNextImage(ulong timeout, VkSemaphore? semaphore, VkFence? fence) =>
        this.extension.AcquireNextImage(this, timeout, semaphore, fence);

    /// <inheritdoc cref="VkSwapchainExtensionKHR.GetSwapchainImages" />
    public VkImage[] GetImages() =>
        this.extension.GetSwapchainImages(this);
}
