using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Extensions.EXT.Flags;
using Age.Vulkan.Native.Extensions.KHR.Enums;
using Age.Vulkan.Native.Flags;
using Age.Vulkan.Native.Types;

namespace Age.Vulkan.Native.Extensions.KHR.Types;

/// <summary>
/// <para>Structure specifying parameters of a newly created swapchain object.</para>
/// <para>Upon calling <see cref="VkKhrSwapchain.CreateSwapchain"/> with an oldSwapchain that is not VK_NULL_HANDLE, oldSwapchain is retired — even if creation of the new swapchain fails. The new swapchain is created in the non-retired state whether or not oldSwapchain is VK_NULL_HANDLE.</para>
/// <para>Upon calling <see cref="VkKhrSwapchain.CreateSwapchain"/> with an oldSwapchain that is not VK_NULL_HANDLE, any images from oldSwapchain that are not acquired by the application may be freed by the implementation, which may occur even if creation of the new swapchain fails. The application can destroy oldSwapchain to free all memory associated with oldSwapchain.</para>
/// <remarks>
/// <para>Multiple retired swapchains can be associated with the same <see cref="VkSurfaceKHR"/> through multiple uses of oldSwapchain that outnumber calls to <see cref="VkKhrSwapchain.DestroySwapchain"/>.</para>
/// <para>After oldSwapchain is retired, the application can pass to <see cref="VkKhrSwapchain.QueuePresent"/> any images it had already acquired from oldSwapchain. E.g., an application may present an image from the old swapchain before an image from the new swapchain is ready to be presented. As usual, vkQueuePresentKHR may fail if oldSwapchain has entered a state that causes <see cref="VkResult.VK_ERROR_OUT_OF_DATE_KHR"/> to be returned.</para>
/// <para>The application can continue to use a shared presentable image obtained from oldSwapchain until a presentable image is acquired from the new swapchain, as long as it has not entered a state that causes it to return VK_ERROR_OUT_OF_DATE_KHR.</para>
/// </remarks>
/// </summary>
public unsafe struct VkSwapchainCreateInfoKHR
{
    /// <summary>
    /// A VkStructureType value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Is NULL or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask of VkSwapchainCreateFlagBitsKHR indicating parameters of the swapchain creation.
    /// </summary>
    public VkSwapchainCreateFlagsKHR flags;

    /// <summary>
    /// The surface onto which the swapchain will present images. If the creation succeeds, the swapchain becomes associated with surface.
    /// </summary>
    public VkSurfaceKHR surface;

    /// <summary>
    /// The minimum number of presentable images that the application needs. The implementation will either create the swapchain with at least that many images, or it will fail to create the swapchain.
    /// </summary>
    public uint minImageCount;

    /// <summary>
    /// A <see cref="VkFormat"/> value specifying the format the swapchain image(s) will be created with.
    /// </summary>
    public VkFormat imageFormat;

    /// <summary>
    /// A <see cref="VkColorSpaceKHR"/> value specifying the way the swapchain interprets image data.
    /// </summary>
    public VkColorSpaceKHR imageColorSpace;

    /// <summary>
    /// The size (in pixels) of the swapchain image(s). The behavior is platform-dependent if the image extent does not match the surface’s currentExtent as returned by <see cref="VkKhrSurface.GetPhysicalDeviceSurfaceCapabilities"/>.
    /// </summary>
    /// <remarks>On some platforms, it is normal that maxImageExtent may become (0, 0), for example when the window is minimized. In such a case, it is not possible to create a swapchain due to the Valid Usage requirements , unless scaling is selected through <see cref="VkSwapchainPresentScalingCreateInfoEXT"/>, if supported .</remarks>
    public VkExtent2D imageExtent;

    /// <summary>
    /// The number of views in a multiview/stereo surface. For non-stereoscopic-3D applications, this value is 1.
    /// </summary>
    public uint imageArrayLayers;

    /// <summary>
    /// A bitmask of <see cref="VkImageUsageFlagBits"/> describing the intended usage of the (acquired) swapchain images.
    /// </summary>
    public VkImageUsageFlags imageUsage;

    /// <summary>
    /// The sharing mode used for the image(s) of the swapchain.
    /// </summary>
    public VkSharingMode imageSharingMode;

    /// <summary>
    /// The number of queue families having access to the image(s) of the swapchain when imageSharingMode is <see cref="VkSharingMode.VK_SHARING_MODE_CONCURRENT"/>.
    /// </summary>
    public uint queueFamilyIndexCount;

    /// <summary>
    /// A pointer to an array of queue family indices having access to the images(s) of the swapchain when imageSharingMode is <see cref="VkSharingMode.VK_SHARING_MODE_CONCURRENT"/>.
    /// </summary>
    public uint* pQueueFamilyIndices;

    /// <summary>
    /// A <see cref="VkSurfaceTransformFlagBitsKHR"/> value describing the transform, relative to the presentation engine’s natural orientation, applied to the image content prior to presentation. If it does not match the currentTransform value returned by vkGetPhysicalDeviceSurfaceCapabilitiesKHR, the presentation engine will transform the image content as part of the presentation operation.
    /// </summary>
    public VkSurfaceTransformFlagBitsKHR preTransform;

    /// <summary>
    /// A <see cref="VkCompositeAlphaFlagBitsKHR"/> value indicating the alpha compositing mode to use when this surface is composited together with other surfaces on certain window systems.
    /// </summary>
    public VkCompositeAlphaFlagBitsKHR compositeAlpha;

    /// <summary>
    /// Is the presentation mode the swapchain will use. A swapchain’s present mode determines how incoming present requests will be processed and queued internally.
    /// </summary>
    public VkPresentModeKHR presentMode;

    /// <summary>
    /// <para>Specifies whether the Vulkan implementation is allowed to discard rendering operations that affect regions of the surface that are not visible.</para>
    /// <list type="bullet">
    /// <item>If set to VK_TRUE, the presentable images associated with the swapchain may not own all of their pixels. Pixels in the presentable images that correspond to regions of the target surface obscured by another window on the desktop, or subject to some other clipping mechanism will have undefined content when read back. Fragment shaders may not execute for these pixels, and thus any side effects they would have had will not occur. Setting VK_TRUE does not guarantee any clipping will occur, but allows more efficient presentation methods to be used on some platforms.</item>
    /// <item>
    /// <para>If set to VK_FALSE, presentable images associated with the swapchain will own all of the pixels they contain.</para>
    /// <remarks>Applications should set this value to VK_TRUE if they do not expect to read back the content of presentable images before presenting them or after reacquiring them, and if their fragment shaders do not have any side effects that require them to run for all pixels in the presentable image.</remarks>
    /// </item>
    /// </list>
    /// </summary>
    public VkBool32 clipped;

    /// <summary>
    /// VK_NULL_HANDLE, or the existing non-retired swapchain currently associated with surface. Providing a valid oldSwapchain may aid in the resource reuse, and also allows the application to still present any images that are already acquired from it.
    /// </summary>
    public VkSwapchainKHR oldSwapchain;

    public VkSwapchainCreateInfoKHR() =>
        sType = VkStructureType.VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR;
}
