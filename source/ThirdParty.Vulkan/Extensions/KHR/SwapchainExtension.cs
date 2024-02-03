using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.KHR;
using ThirdParty.Vulkan.Native;
using ThirdParty.Vulkan.Native.KHR;

namespace ThirdParty.Vulkan.Extensions.KHR;

public unsafe class SwapchainExtension : IDeviceExtension<SwapchainExtension>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkAcquireNextImageKHR(VkDevice device, VkSwapchainKHR swapchain, ulong timeout, VkSemaphore semaphore, VkFence fence, uint* pImageIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateSwapchainKHR(VkDevice device, VkSwapchainCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchain);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroySwapchainKHR(VkDevice device, VkSwapchainKHR swapchain, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkGetSwapchainImagesKHR(VkDevice device, VkSwapchainKHR swapchain, uint* pSwapchainImageCount, VkImage* pSwapchainImages);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkQueuePresentKHR(VkQueue queue, VkPresentInfoKHR* pPresentInfo);

    public static string Name { get; } = "VK_KHR_swapchain";

    private readonly Device                  device;
    private readonly VkAcquireNextImageKHR   vkAcquireNextImageKHR;
    private readonly VkCreateSwapchainKHR    vkCreateSwapchainKHR;
    private readonly VkDestroySwapchainKHR   vkDestroySwapchainKHR;
    private readonly VkGetSwapchainImagesKHR vkGetSwapchainImagesKHR;
    private readonly VkQueuePresentKHR       vkQueuePresentKHR;

    internal SwapchainExtension(Device device)
    {
        this.device = device;

        this.vkAcquireNextImageKHR   = device.GetProcAddr<VkAcquireNextImageKHR>(nameof(this.vkAcquireNextImageKHR));
        this.vkCreateSwapchainKHR    = device.GetProcAddr<VkCreateSwapchainKHR>(nameof(this.vkCreateSwapchainKHR));
        this.vkDestroySwapchainKHR   = device.GetProcAddr<VkDestroySwapchainKHR>(nameof(this.vkDestroySwapchainKHR));
        this.vkGetSwapchainImagesKHR = device.GetProcAddr<VkGetSwapchainImagesKHR>(nameof(this.vkGetSwapchainImagesKHR));
        this.vkQueuePresentKHR       = device.GetProcAddr<VkQueuePresentKHR>(nameof(this.vkQueuePresentKHR));
    }

    static SwapchainExtension IDeviceExtension<SwapchainExtension>.Create(Device device) =>
        new(device);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkAcquireNextImageKHR.html">vkAcquireNextImageKHR</see>
    /// </summary>
    public uint AcquireNextImage(Swapchain swapchain, ulong timeout, VkSemaphore semaphore, VkFence fence)
    {
        uint imageIndex;

        VulkanException.Check(this.vkAcquireNextImageKHR.Invoke(this.device, swapchain, timeout, semaphore, fence, &imageIndex));

        return imageIndex;
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateSwapchainKHR.html">vkCreateSwapchainKHR</see>
    /// </summary>
    public Swapchain CreateSwapchain(Swapchain.CreateInfo createInfo)
    {
        VkSwapchainKHR swapchain;

        VulkanException.Check(this.vkCreateSwapchainKHR.Invoke(this.device, createInfo, this.device.PhysicalDevice.Instance.Allocator, &swapchain));

        return new(swapchain, this);
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroySwapchainKHR.html">vkDestroySwapchainKHR</see>
    /// </summary>
    public void DestroySwapchain(Swapchain swapchain) =>
        this.vkDestroySwapchainKHR.Invoke(this.device, swapchain, this.device.PhysicalDevice.Instance.Allocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetSwapchainImagesKHR.html">vkGetSwapchainImagesKHR</see>
    /// </summary>
    public Image[] GetSwapchainImages(Swapchain swapchain)
    {
        uint swapchainImageCount;

        VulkanException.Check(this.vkGetSwapchainImagesKHR.Invoke(this.device, swapchain, &swapchainImageCount, null));

        var swapchainImages = new VkImage[swapchainImageCount];

        fixed (VkImage* pSwapchainImages = swapchainImages)
        {
            VulkanException.Check(this.vkGetSwapchainImagesKHR.Invoke(this.device, swapchain, &swapchainImageCount, pSwapchainImages));
        }

        var images = new Image[swapchainImageCount];

        for (var i = 0; i < swapchainImageCount; i++)
        {
            images[i] = new(swapchainImages[i], this.device, true);
        }

        return images;
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkQueuePresentKHR.html">vkQueuePresentKHR</see>
    /// </summary>
    public void QueuePresent(Queue queue, PresentInfo presentInfo) =>
        VulkanException.Check(this.vkQueuePresentKHR.Invoke(queue, presentInfo));
}
