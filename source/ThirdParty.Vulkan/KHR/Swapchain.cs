using ThirdParty.Vulkan.Extensions.KHR;

namespace ThirdParty.Vulkan.KHR;

public partial class Swapchain : DisposableNativeHandle
{
    private readonly SwapchainExtension extension;

    internal Swapchain(VkSwapchainKHR handle, SwapchainExtension extension) : base(handle) =>
        this.extension = extension;

    protected override void OnDispose() =>
        this.extension.DestroySwapchain(this);

    /// <inheritdoc cref="SwapchainExtension.AcquireNextImage" />
    public uint AcquireNextImage(ulong timeout, Semaphore? semaphore, Fence? fence) =>
        this.extension.AcquireNextImage(this, timeout, semaphore, fence);

    /// <inheritdoc cref="SwapchainExtension.GetSwapchainImages" />
    public Image[] GetImages() =>
        this.extension.GetSwapchainImages(this);
}
