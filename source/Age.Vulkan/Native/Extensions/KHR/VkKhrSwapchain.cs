using System.Runtime.InteropServices;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Types;
using Age.Vulkan.Native.Types.KHR;

using static Age.Core.Unsafe.UnmanagedUtils;

namespace Age.Vulkan.Native.Extensions.KHR;

/// <remarks>Provided by VK_KHR_swapchain</remarks>
public unsafe class VkKhrSwapchain(Vk vk, VkDevice device) : IVkDeviceExtension
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateSwapchainKHR(VkDevice device, VkSwapchainCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchain);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroySwapchainKHR(VkDevice device, VkSwapchainKHR swapchain, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkGetSwapchainImagesKHR(VkDevice device, VkSwapchainKHR swapchain, uint* pSwapchainImageCount, VkImage* pSwapchainImages);

    public static string Name { get; } = "VK_KHR_swapchain";

    private readonly VkCreateSwapchainKHR    vkCreateSwapchainKHR    = vk.GetDeviceProcAddr<VkCreateSwapchainKHR>(Name, device, "vkCreateSwapchainKHR");
    private readonly VkDestroySwapchainKHR   vkDestroySwapchainKHR   = vk.GetDeviceProcAddr<VkDestroySwapchainKHR>(Name, device, "vkDestroySwapchainKHR");
    private readonly VkGetSwapchainImagesKHR vkGetSwapchainImagesKHR = vk.GetDeviceProcAddr<VkGetSwapchainImagesKHR>(Name, device, "vkGetSwapchainImagesKHR");

    public static IVkDeviceExtension Create(Vk vk, VkDevice device) =>
        new VkKhrSwapchain(vk, device);

    /// <summary>
    /// <para>Retrieve the index of the next available presentable image.</para>
    /// <para>If the swapchain has been created with the <see cref="VkSwapchainCreateFlagBitsKHR.VK_SWAPCHAIN_CREATE_DEFERRED_MEMORY_ALLOCATION_BIT_EXT"/> flag, the image whose index is returned in pImageIndex will be fully backed by memory before this call returns to the application, as if it is bound completely and contiguously to a single <see cref="VkDeviceMemory"/> object.</para>
    /// </summary>
    /// <param name="device"></param>
    /// <param name="swapchain"></param>
    /// <param name="timeout"></param>
    /// <param name="semaphore"></param>
    /// <param name="fence"></param>
    /// <param name="pImageIndex"></param>
    public VkResult AcquireNextImageKHR(VkDevice device, VkSwapchainKHR swapchain, ulong timeout, VkSemaphore semaphore, VkFence fence, uint* pImageIndex) => throw new NotImplementedException();

    /// <summary>
    /// <para>Create a swapchain.</para>
    /// <para>As mentioned above, if <see cref="CreateSwapchain"/> succeeds, it will return a handle to a swapchain containing an array of at least pCreateInfo->minImageCount presentable images.</para>
    /// <para>While acquired by the application, presentable images can be used in any way that equivalent non-presentable images can be used. A presentable image is equivalent to a non-presentable image created with the following <see cref="VkImageCreateInfo"/> parameters:</para>
    /// <list type="table">
    /// <listheader>
    /// <term><see cref="VkImageCreateInfo"/> Field</term>
    /// <description>Value</description>
    /// </listheader>
    /// <item>
    /// <term>flags</term>
    /// <description>
    /// <para><see cref="VkImageCreate.VK_IMAGE_CREATE_SPLIT_INSTANCE_BIND_REGIONS_BIT"/> is set if <see cref="VkSwapchainCreate.VK_SWAPCHAIN_CREATE_SPLIT_INSTANCE_BIND_REGIONS_BIT_KHR"/> is set</para>
    /// <para><see cref="VkImageCreate.VK_IMAGE_CREATE_PROTECTED_BIT"/> is set if <see cref="VkSwapchainCreate.VK_SWAPCHAIN_CREATE_PROTECTED_BIT_KHR"/> is set</para>
    /// <para><see cref="VkImageCreate.VK_IMAGE_CREATE_MUTABLE_FORMAT_BIT"/> and <see cref="VkImageCreate.VK_IMAGE_CREATE_EXTENDED_USAGE_BIT_KHR"/> are both set if <see cref="VkSwapchainCreate.VK_SWAPCHAIN_CREATE_MUTABLE_FORMAT_BIT_KHR"/> is set</para>
    /// <para>all other bits are unset</para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>imageType</term>
    /// <description>VK_IMAGE_TYPE_2D</description>
    /// </item>
    /// <item>format
    /// <description>pCreateInfo->imageFormat</description>
    /// </item>
    /// <item>
    /// <term>extent</term>
    /// <description>{pCreateInfo->imageExtent.width, pCreateInfo->imageExtent.height, 1}</description>
    /// </item>
    /// <item>
    /// <term>mipLevels</term>
    /// <description>1</description>
    /// </item>
    /// <item>
    /// <term>arrayLayers</term>
    /// <description>pCreateInfo->imageArrayLayers</description>
    /// </item>
    /// <item>
    /// <term>samples</term>
    /// <description><see cref="VkSampleCount.VK_SAMPLE_COUNT_1_BIT"/></description>
    /// </item>
    /// <item>
    /// <term>tiling</term>
    /// <description><see cref="VkImageTiling.VK_IMAGE_TILING_OPTIMAL"/></description>
    /// </item>
    /// <item>
    /// <term>usage</term>
    /// <description>pCreateInfo->imageUsage</description>
    /// </item>
    /// <item>
    /// <term>sharingMode</term>
    /// <description>pCreateInfo->imageSharingMode</description>
    /// </item>
    /// <item>
    /// <term>queueFamilyIndexCount</term>
    /// <description>pCreateInfo->queueFamilyIndexCount</description>
    /// </item>
    /// <item>
    /// <term>pQueueFamilyIndices</term>
    /// <description>pCreateInfo->pQueueFamilyIndices</description>
    /// </item>
    /// <item>
    /// <term>initialLayout</term>
    /// <description>VK_IMAGE_LAYOUT_UNDEFINED</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="device">The device to create the swapchain for.</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkSwapchainCreateInfoKHR"/> structure specifying the parameters of the created swapchain.</param>
    /// <param name="pAllocator">The allocator used for host memory allocated for the swapchain object when there is no more specific allocator available (see <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see>).</param>
    /// <param name="pSwapchain">A pointer to a <see cref="VkSwapchainKHR"/> handle in which the created swapchain object will be returned.</param>
    public VkResult CreateSwapchain(VkDevice device, VkSwapchainCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchain) =>
        this.vkCreateSwapchainKHR.Invoke(device, pCreateInfo, pAllocator, pSwapchain);

    public VkResult CreateSwapchain(VkDevice device, in VkSwapchainCreateInfoKHR createInfo, in VkAllocationCallbacks allocator, out VkSwapchainKHR swapchain)
    {
        fixed (VkSwapchainCreateInfoKHR* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*    pAllocator  = &allocator)
        fixed (VkSwapchainKHR*           pSwapchain  = &swapchain)
        {
            return this.vkCreateSwapchainKHR.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pSwapchain);
        }
    }

    /// <summary>
    /// <para>Destroy a swapchain object.</para>
    /// <para>The application must not destroy a swapchain until after completion of all outstanding operations on images that were acquired from the swapchain. swapchain and all associated VkImage handles are destroyed, and must not be acquired or used any more by the application. The memory of each <see cref="VkImage"/> will only be freed after that image is no longer used by the presentation engine. For example, if one image of the swapchain is being displayed in a window, the memory for that image may not be freed until the window is destroyed, or another swapchain is created for the window. Destroying the swapchain does not invalidate the parent <see cref="VkSurfaceKHR"/>, and a new swapchain can be created with it.</para>
    /// <para>When a swapchain associated with a display surface is destroyed, if the image most recently presented to the display surface is from the swapchain being destroyed, then either any display resources modified by presenting images from any swapchain associated with the display surface must be reverted by the implementation to their state prior to the first present performed on one of these swapchains, or such resources must be left in their current state.</para>
    /// <para>If swapchain has exclusive full-screen access, it is released before the swapchain is destroyed.</para>
    /// </summary>
    /// <param name="device">The <see cref="VkDevice"/> associated with swapchain.</param>
    /// <param name="swapchain">The swapchain to destroy.</param>
    /// <param name="pAllocator">The allocator used for host memory allocated for the swapchain object when there is no more specific allocator available (see <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see>).</param>
    public void DestroySwapchain(VkDevice device, VkSwapchainKHR swapchain, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroySwapchainKHR.Invoke(device, swapchain, pAllocator);

    /// <summary>
    /// <para>Obtain the array of presentable images associated with a swapchain</para>
    /// <para>If pSwapchainImages is NULL, then the number of presentable images for swapchain is returned in pSwapchainImageCount. Otherwise, pSwapchainImageCount must point to a variable set by the user to the number of elements in the pSwapchainImages array, and on return the variable is overwritten with the number of structures actually written to pSwapchainImages. If the value of pSwapchainImageCount is less than the number of presentable images for swapchain, at most pSwapchainImageCount structures will be written, and <see cref="VkResult.VK_INCOMPLETE"/> will be returned instead of <see cref="VkResult.VK_SUCCESS"/>, to indicate that not all the available presentable images were returned.</para>
    /// </summary>
    /// <param name="device">the device associated with swapchain.</param>
    /// <param name="swapchain">the swapchain to query.</param>
    /// <param name="pSwapchainImageCount">a pointer to an integer related to the number of presentable images available or queried, as described below.</param>
    /// <param name="pSwapchainImages">either NULL or a pointer to an array of <see cref="VkImage"/> handles.</param>
    /// <returns></returns>
    public VkResult GetSwapchainImages(VkDevice device, VkSwapchainKHR swapchain, uint* pSwapchainImageCount, VkImage* pSwapchainImages) =>
        this.vkGetSwapchainImagesKHR.Invoke(device, swapchain, pSwapchainImageCount, pSwapchainImages);

    public VkResult GetSwapchainImages(VkDevice device, VkSwapchainKHR swapchain, out uint swapchainImageCount)
    {
        fixed (uint* pSwapchainImageCount = &swapchainImageCount)
        {
            return this.vkGetSwapchainImagesKHR.Invoke(device, swapchain, pSwapchainImageCount, null);
        }
    }

    public VkResult GetSwapchainImages(VkDevice device, VkSwapchainKHR swapchain, out VkImage[] swapchainImages)
    {
        if (this.GetSwapchainImages(device, swapchain, out uint swapchainImageCount) is var result and not VkResult.VK_SUCCESS)
        {
            swapchainImages = Array.Empty<VkImage>();

            return result;
        }

        swapchainImages = new VkImage[swapchainImageCount];

        fixed (VkImage* pSwapchainImages = swapchainImages)
        {
            return this.vkGetSwapchainImagesKHR.Invoke(device, swapchain, &swapchainImageCount, pSwapchainImages);
        }
    }

    /// <summary>
    /// <para>Queue an image for presentation.</para>
    /// <para>Note: There is no requirement for an application to present images in the same order that they were acquired - applications can arbitrarily present any image that is currently acquired.</para>
    /// </summary>
    /// <param name="queue">A queue that is capable of presentation to the target surface’s platform on the same device as the image’s swapchain.</param>
    /// <param name="pPresentInfo">A pointer to a <see cref="VkPresentInfoKHR"/> structure specifying parameters of the presentation.</param>
    public VkResult QueuePresent(VkQueue queue, VkPresentInfoKHR* pPresentInfo) => throw new NotImplementedException();
}
