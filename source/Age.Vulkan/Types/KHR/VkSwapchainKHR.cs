using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Types.KHR;

/// <summary>
/// <para>Opaque handle to a swapchain object.</para>
/// <para>A swapchain is an abstraction for an array of presentable images that are associated with a surface. The presentable images are represented by VkImage objects created by the platform. One image (which can be an array image for multiview/stereoscopic-3D surfaces) is displayed at a time, but multiple images can be queued for presentation. An application renders to the image, and then queues the image for presentation to the surface.</para>
/// <para>A native window cannot be associated with more than one non-retired swapchain at a time. Further, swapchains cannot be created for native windows that have a non-Vulkan graphics API surface associated with them.</para>
/// <remarks>The presentation engine is an abstraction for the platform’s compositor or display engine.</remarks>
/// <para>The presentation engine may be synchronous or asynchronous with respect to the application and/or logical device.</para>
/// <para>Some implementations may use the device’s graphics queue or dedicated presentation hardware to perform presentation.</para>
/// <para>The presentable images of a swapchain are owned by the presentation engine. An application can acquire use of a presentable image from the presentation engine. Use of a presentable image must occur only after the image is returned by vkAcquireNextImageKHR, and before it is released by <see cref="VkKhrSwapchain.QueuePresent"/>. This includes transitioning the image layout and rendering commands.</para>
/// <para>An application can acquire use of a presentable image with vkAcquireNextImageKHR. After acquiring a presentable image and before modifying it, the application must use a synchronization primitive to ensure that the presentation engine has finished reading from the image. The application can then transition the image’s layout, queue rendering commands to it, etc. Finally, the application presents the image with <see cref="VkKhrSwapchain.QueuePresent"/>, which releases the acquisition of the image. The application can also release the acquisition of the image through vkReleaseSwapchainImagesEXT, if the image is not in use by the device, and skip the present operation.</para>
/// <para>The presentation engine controls the order in which presentable images are acquired for use by the application.</para>
/// <remarks>This allows the platform to handle situations which require out-of-order return of images after presentation. At the same time, it allows the application to generate command buffers referencing all of the images in the swapchain at initialization time, rather than in its main loop.</remarks>
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkSwapchainKHR(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkSwapchainKHR(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkSwapchainKHR value) => value.Value;
}
