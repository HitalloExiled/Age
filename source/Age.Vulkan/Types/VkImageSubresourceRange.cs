using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure specifying an image subresource range.</para>
/// <para>The number of mipmap levels and array layers must be a subset of the image subresources in the image. If an application wants to use all mip levels or layers in an image after the baseMipLevel or baseArrayLayer, it can set levelCount and layerCount to the special values <see cref="Vk.VK_REMAINING_MIP_LEVELS"/> and <see cref="Vk.VK_REMAINING_ARRAY_LAYERS"/> without knowing the exact number of mip levels or layers.</para>
/// <para>For cube and cube array image views, the layers of the image view starting at baseArrayLayer correspond to faces in the order +X, -X, +Y, -Y, +Z, -Z. For cube arrays, each set of six sequential layers is a single cube, so the number of cube maps in a cube map array view is layerCount / 6, and image array layer (baseArrayLayer + i) is face index (i mod 6) of cube i / 6. If the number of layers in the view, whether set explicitly in layerCount or implied by <see cref="Vk.VK_REMAINING_ARRAY_LAYERS"/>, is not a multiple of 6, the last cube map in the array must not be accessed.</para>
/// <para>aspectMask must be only <see cref="VkImageAspect.VK_IMAGE_ASPECT_COLOR_BIT"/>, <see cref="VkImageAspect.VK_IMAGE_ASPECT_DEPTH_BIT"/> or <see cref="VkImageAspect.VK_IMAGE_ASPECT_STENCIL_BIT"/> if format is a color, depth-only or stencil-only format, respectively, except if format is a multi-planar format. If using a depth/stencil format with both depth and stencil components, aspectMask must include at least one of <see cref="VkImageAspect.VK_IMAGE_ASPECT_DEPTH_BIT"/> and <see cref="VkImageAspect.VK_IMAGE_ASPECT_STENCIL_BIT"/>, and can include both.</para>
/// <para>When the <see cref="VkImageSubresourceRange"/> structure is used to select a subset of the slices of a 3D image’s mip level in order to create a 2D or 2D array image view of a 3D image created with <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_2D_ARRAY_COMPATIBLE_BIT"/>, baseArrayLayer and layerCount specify the first slice index and the number of slices to include in the created image view. Such an image view can be used as a framebuffer attachment that refers only to the specified range of slices of the selected mip level. However, any layout transitions performed on such an attachment view during a render pass instance still apply to the entire subresource referenced which includes all the slices of the selected mip level.</para>
/// <para>When using an image view of a depth/stencil image to populate a descriptor set (e.g. for sampling in the shader, or for use as an input attachment), the aspectMask must only include one bit, which selects whether the image view is used for depth reads (i.e. using a floating-point sampler or input attachment in the shader) or stencil reads (i.e. using an unsigned integer sampler or input attachment in the shader). When an image view of a depth/stencil image is used as a depth/stencil framebuffer attachment, the aspectMask is ignored and both depth and stencil image subresources are used.</para>
/// <para>When creating a <see cref="VkImageView"/>, if sampler Y′CBCR conversion is enabled in the sampler, the aspectMask of a subresourceRange used by the <see cref="VkImageView"/> must be <see cref="VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT"/>.</para>
/// <para>When creating a <see cref="VkImageView"/>, if sampler Y′CBCR conversion is not enabled in the sampler and the image format is multi-planar, the image must have been created with <see cref="VkImageCreateFlagBits.VK_IMAGE_CREATE_MUTABLE_FORMAT_BIT"/>, and the aspectMask of the <see cref="VkImageView"/>’s subresourceRange must be <see cref="VkImageAspect.VK_IMAGE_ASPECT_PLANE_0_BIT"/>, <see cref="VkImageAspect.VK_IMAGE_ASPECT_PLANE_1_BIT"/> or <see cref="VkImageAspect.VK_IMAGE_ASPECT_PLANE_2_BIT"/>.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkImageSubresourceRange
{
    /// <summary>
    /// A bitmask of VkImageAspectFlagBits specifying which aspect(s) of the image are included in the view.
    /// </summary>
    public VkImageAspectFlags aspectMask;

    /// <summary>
    /// The first mipmap level accessible to the view.
    /// </summary>
    public uint baseMipLevel;

    /// <summary>
    /// The number of mipmap levels (starting from baseMipLevel) accessible to the view.
    /// </summary>
    public uint levelCount;

    /// <summary>
    /// The first array layer accessible to the view.
    /// </summary>
    public uint baseArrayLayer;

    /// <summary>
    /// The number of array layers (starting from baseArrayLayer) accessible to the view.
    /// </summary>
    public uint layerCount;
}
