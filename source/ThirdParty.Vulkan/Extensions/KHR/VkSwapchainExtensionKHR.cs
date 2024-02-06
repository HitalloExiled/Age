using System.Runtime.InteropServices;
using Age.Core.Interop;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.KHR;
using ThirdParty.Vulkan.Native;
using ThirdParty.Vulkan.Native.KHR;

namespace ThirdParty.Vulkan.Extensions.KHR;

public unsafe class VkSwapchainExtensionKHR : IDeviceExtension<VkSwapchainExtensionKHR>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkAcquireNextImageKHR(VkHandle<VkDevice> device, VkHandle<VkSwapchainKHR> swapchain, ulong timeout, VkHandle<VkSemaphore> semaphore, VkHandle<VkFence> fence, uint* pImageIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateSwapchainKHR(VkHandle<VkDevice> device, VkSwapchainCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkSwapchainKHR>* pSwapchain);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroySwapchainKHR(VkHandle<VkDevice> device, VkHandle<VkSwapchainKHR> swapchain, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkGetSwapchainImagesKHR(VkHandle<VkDevice> device, VkHandle<VkSwapchainKHR> swapchain, uint* pSwapchainImageCount, VkHandle<VkImage>* pSwapchainImages);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkQueuePresentKHR(VkHandle<VkQueue> queue, VkPresentInfoKHR* pPresentInfo);

    public static string Name { get; } = "VK_KHR_swapchain";

    private readonly VkDevice                  device;
    private readonly VkAcquireNextImageKHR   vkAcquireNextImageKHR;
    private readonly VkCreateSwapchainKHR    vkCreateSwapchainKHR;
    private readonly VkDestroySwapchainKHR   vkDestroySwapchainKHR;
    private readonly VkGetSwapchainImagesKHR vkGetSwapchainImagesKHR;
    private readonly VkQueuePresentKHR       vkQueuePresentKHR;

    internal VkSwapchainExtensionKHR(VkDevice device)
    {
        this.device = device;

        this.vkAcquireNextImageKHR   = device.GetProcAddr<VkAcquireNextImageKHR>(nameof(this.vkAcquireNextImageKHR));
        this.vkCreateSwapchainKHR    = device.GetProcAddr<VkCreateSwapchainKHR>(nameof(this.vkCreateSwapchainKHR));
        this.vkDestroySwapchainKHR   = device.GetProcAddr<VkDestroySwapchainKHR>(nameof(this.vkDestroySwapchainKHR));
        this.vkGetSwapchainImagesKHR = device.GetProcAddr<VkGetSwapchainImagesKHR>(nameof(this.vkGetSwapchainImagesKHR));
        this.vkQueuePresentKHR       = device.GetProcAddr<VkQueuePresentKHR>(nameof(this.vkQueuePresentKHR));
    }

    static VkSwapchainExtensionKHR IDeviceExtension<VkSwapchainExtensionKHR>.Create(VkDevice device) =>
        new(device);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkAcquireNextImageKHR.html">vkAcquireNextImageKHR</see>
    /// </summary>
    public uint AcquireNextImage(VkSwapchainKHR swapchain, ulong timeout, VkSemaphore? semaphore, VkFence? fence)
    {
        uint imageIndex;

        VkException.Check(this.vkAcquireNextImageKHR.Invoke(this.device, swapchain, timeout, semaphore, fence, &imageIndex));

        return imageIndex;
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateSwapchainKHR.html">vkCreateSwapchainKHR</see>
    /// </summary>
    public VkSwapchainKHR CreateSwapchain(in VkSwapchainCreateInfoKHR createInfo)
    {
        VkHandle<VkSwapchainKHR> swapchain;

        fixed (VkSwapchainCreateInfoKHR* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*    pAllocator  = &this.device.Instance.Allocator)
        {
            VkException.Check(this.vkCreateSwapchainKHR.Invoke(this.device, pCreateInfo, PointerHelper.NullIfDefault(this.device.Instance.Allocator, pAllocator), &swapchain));
        }

        return new(swapchain, this);
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroySwapchainKHR.html">vkDestroySwapchainKHR</see>
    /// </summary>
    public void DestroySwapchain(VkSwapchainKHR swapchain)
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.device.Instance.Allocator)
        {
            this.vkDestroySwapchainKHR.Invoke(this.device, swapchain, PointerHelper.NullIfDefault(this.device.Instance.Allocator, pAllocator));
        }
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetSwapchainImagesKHR.html">vkGetSwapchainImagesKHR</see>
    /// </summary>
    public VkImage[] GetSwapchainImages(VkSwapchainKHR swapchain)
    {
        uint swapchainImageCount;

        VkException.Check(this.vkGetSwapchainImagesKHR.Invoke(this.device, swapchain, &swapchainImageCount, null));

        var swapchainImages = new VkHandle<VkImage>[swapchainImageCount];

        fixed (VkHandle<VkImage>* pSwapchainImages = swapchainImages)
        {
            VkException.Check(this.vkGetSwapchainImagesKHR.Invoke(this.device, swapchain, &swapchainImageCount, pSwapchainImages));
        }

        var images = new VkImage[swapchainImageCount];

        for (var i = 0; i < swapchainImageCount; i++)
        {
            images[i] = new(swapchainImages[i], this.device, true);
        }

        return images;
    }

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkQueuePresentKHR.html">vkQueuePresentKHR</see>
    /// </summary>
    public void QueuePresent(VkQueue queue, in VkPresentInfoKHR presentInfo)
    {
        fixed (VkPresentInfoKHR* pPresentInfo = &presentInfo)
        {
            VkException.Check(this.vkQueuePresentKHR.Invoke(queue, pPresentInfo));
        }
    }
}
