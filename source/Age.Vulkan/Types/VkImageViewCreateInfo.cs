using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Some of the image creation parameters are inherited by the view. In particular, image view creation inherits the implicit parameter usage specifying the allowed usages of the image view that, by default, takes the value of the corresponding usage parameter specified in <see cref="VkImageCreateInfo"/> at image creation time. The implicit usage can be overridden by adding a <see cref="VkImageViewUsageCreateInfo"/> structure to the pNext chain, but the view usage must be a subset of the image usage. If image has a depth-stencil format and was created with a <see cref="VkImageStencilUsageCreateInfo"/> structure included in the pNext chain of <see cref="VkImageCreateInfo"/>, the usage is calculated based on the subresource.aspectMask provided:</para>
/// <list type="bullet">
/// <item>If aspectMask includes only <see cref="VkImageAspect.VK_IMAGE_ASPECT_STENCIL_BIT"/>, the implicit usage is equal to <see cref="VkImageStencilUsageCreateInfo.stencilUsage"/>.</item>
/// <item>If aspectMask includes only <see cref="VkImageAspect.VK_IMAGE_ASPECT_DEPTH_BIT"/>, the implicit usage is equal to <see cref="VkImageCreateInfo.usage"/>.</item>
/// <item>If both aspects are included in aspectMask, the implicit usage is equal to the intersection of <see cref="VkImageCreateInfo.usage"/> and <see cref="VkImageStencilUsageCreateInfo.stencilUsage"/>.</item>
/// </list>
/// <para>If image is a 3D image, its Z range can be restricted to a subset by adding a <see cref="VkImageViewSlicedCreateInfoEXT"/> to the pNext chain.</para>
/// <para>If image was created with the <see cref="VkImageCreate.VK_IMAGE_CREATE_MUTABLE_FORMAT_BIT"/> flag, and if the format of the image is not multi-planar, format can be different from the image’s format, but if image was created without the <see cref="VkImageCreate.VK_IMAGE_CREATE_BLOCK_TEXEL_VIEW_COMPATIBLE_BIT"/> flag and they are not equal they must be compatible. Image format compatibility is defined in the Format Compatibility Classes section. Views of compatible formats will have the same mapping between texel coordinates and memory locations irrespective of the format, with only the interpretation of the bit pattern changing.</para>
/// <para>If image was created with a multi-planar format, and the image view’s aspectMask is one of <see cref="VkImageAspect.VK_IMAGE_ASPECT_PLANE_0_BIT"/>, <see cref="VkImageAspect.VK_IMAGE_ASPECT_PLANE_1_BIT"/> or <see cref="VkImageAspect.VK_IMAGE_ASPECT_PLANE_2_BIT"/>, the view’s aspect mask is considered to be equivalent to <see cref="VkImageAspect.VK_IMAGE_ASPECT_COLOR_BIT"/> when used as a framebuffer attachment.</para>
/// <para>Values intended to be used with one view format may not be exactly preserved when written or read through a different format. For example, an integer value that happens to have the bit pattern of a floating point denorm or NaN may be flushed or canonicalized when written or read through a view with a floating point format. Similarly, a value written through a signed normalized format that has a bit pattern exactly equal to -2b may be changed to -2b + 1 as described in Conversion from Normalized Fixed-Point to Floating-Point.</para>
/// <para>If image was created with the <see cref="VkImageCreate.VK_IMAGE_CREATE_BLOCK_TEXEL_VIEW_COMPATIBLE_BIT"/> flag, format must be compatible with the image’s format as described above; or must be an uncompressed format, in which case it must be size-compatible with the image’s format. In this case, the resulting image view’s texel dimensions equal the dimensions of the selected mip level divided by the compressed texel block size and rounded up.</para>
/// <para>The <see cref="VkComponentMapping"/> components member describes a remapping from components of the image to components of the vector returned by shader image instructions. This remapping must be the identity swizzle for storage image descriptors, input attachment descriptors, framebuffer attachments, and any <see cref="VkImageView"/> used with a combined image sampler that enables sampler Y′CBCR conversion.</para>
/// <para>If the image view is to be used with a sampler which supports sampler Y′CBCR conversion, an identically defined object of type <see cref="VkSamplerYcbcrConversion"/> to that used to create the sampler must be passed to <see cref="Vk.CreateImageView"/> in a <see cref="VkSamplerYcbcrConversionInfo"/> included in the pNext chain of <see cref="VkImageViewCreateInfo"/>. Conversely, if a <see cref="VkSamplerYcbcrConversion"/> object is passed to <see cref="Vk.CreateImageView"/>, an identically defined <see cref="VkSamplerYcbcrConversion"/> object must be used when sampling the image.</para>
/// <para>If the image has a multi-planar format, subresourceRange.aspectMask is <see cref="VkImageAspect.VK_IMAGE_ASPECT_COLOR_BIT"/>, and usage includes <see cref="VkImageUsageFlagBits.VK_IMAGE_USAGE_SAMPLED_BIT"/>, then the format must be identical to the image format and the sampler to be used with the image view must enable sampler Y′CBCR conversion.</para>
/// <para>When such an image is used in a video coding operation, the sampler Y′CBCR conversion has no effect.</para>
/// <para>If image was created with the <see cref="VkImageCreate.VK_IMAGE_CREATE_MUTABLE_FORMAT_BIT"/> and the image has a multi-planar format, and if subresourceRange.aspectMask is <see cref="VkImageAspect.VK_IMAGE_ASPECT_PLANE_0_BIT"/>, <see cref="VkImageAspect.VK_IMAGE_ASPECT_PLANE_1_BIT"/>, or <see cref="VkImageAspect.VK_IMAGE_ASPECT_PLANE_2_BIT"/>, format must be compatible with the corresponding plane of the image, and the sampler to be used with the image view must not enable sampler Y′CBCR conversion. The width and height of the single-plane image view must be derived from the multi-planar image’s dimensions in the manner listed for plane compatibility for the plane.</para>
/// <para>Any view of an image plane will have the same mapping between texel coordinates and memory locations as used by the components of the color aspect, subject to the formulae relating texel coordinates to lower-resolution planes as described in Chroma Reconstruction. That is, if an R or B plane has a reduced resolution relative to the G plane of the multi-planar image, the image view operates using the (uplane, vplane) unnormalized coordinates of the reduced-resolution plane, and these coordinates access the same memory locations as the (ucolor, vcolor) unnormalized coordinates of the color aspect for which chroma reconstruction operations operate on the same (uplane, vplane) or (iplane, jplane) coordinates.</para>
/// Image View Type	Compatible Image Types
/// <list type="table">
/// <listheader>
/// <term>Image View Type</term>
/// <description>Compatible Image Types</description>
/// </listheader>
/// <item>
/// <term>VK_IMAGE_VIEW_TYPE_1D</term>
/// <description>VK_IMAGE_TYPE_1D</description>
/// </item>
/// <item>
/// <term>VK_IMAGE_VIEW_TYPE_1D_ARRAY</term>
/// <description>VK_IMAGE_TYPE_1D</description>
/// </item>
/// <item>
/// <term>VK_IMAGE_VIEW_TYPE_2D</term>
/// <description>VK_IMAGE_TYPE_2D</description>
/// </item>
/// <item>
/// <term>VK_IMAGE_TYPE_3D</term>
/// <description>VK_IMAGE_VIEW_TYPE_2D_ARRAY</description>
/// </item>
/// <item>
/// <term>VK_IMAGE_TYPE_2D</term>
/// <description>VK_IMAGE_TYPE_3D</description>
/// </item>
/// <item>
/// <term>VK_IMAGE_VIEW_TYPE_CUBE</term>
/// <description>VK_IMAGE_TYPE_2D</description>
/// </item>
/// <item>
/// <term>VK_IMAGE_VIEW_TYPE_CUBE_ARRAY</term>
/// <description>VK_IMAGE_TYPE_2D</description>
/// </item>
/// <item>
/// <term>VK_IMAGE_VIEW_TYPE_3D</term>
/// <description>VK_IMAGE_TYPE_3D</description>
/// </item>
/// </list>
/// </summary>
public unsafe struct VkImageViewCreateInfo
{
    /// <summary>
    /// a <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// a bitmask of <see cref="VkImageViewCreateFlagBits"/> specifying additional parameters of the image view.
    /// </summary>
    public VkImageViewCreateFlags flags;

    /// <summary>
    /// a <see cref="VkImage"/> on which the view will be created.
    /// </summary>
    public VkImage image;

    /// <summary>
    /// a <see cref="VkImageViewType"/> value specifying the type of the image view.
    /// </summary>
    public VkImageViewType viewType;

    /// <summary>
    /// a <see cref="VkFormat"/> specifying the format and type used to interpret texel blocks of the image.
    /// </summary>
    public VkFormat format;

    /// <summary>
    /// a <see cref="VkComponentMapping"/> structure specifying a remapping of color components (or of depth or stencil components after they have been converted into color components).
    /// </summary>
    public VkComponentMapping components;

    /// <summary>
    /// a <see cref="VkImageSubresourceRange"/> structure selecting the set of mipmap levels and array layers to be accessible to the view.
    /// </summary>
    public VkImageSubresourceRange subresourceRange;

    public VkImageViewCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO;
}
