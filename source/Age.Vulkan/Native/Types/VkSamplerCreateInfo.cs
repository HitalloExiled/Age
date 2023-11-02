using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying parameters of a newly created sampler.</para>
/// <para>Mapping of OpenGL to Vulkan filter modes</para>
/// <para>magFilter values of <see cref="VkFilter.VK_FILTER_NEAREST"/> and <see cref="VkFilter.VK_FILTER_LINEAR"/> directly correspond to GL_NEAREST and GL_LINEAR magnification filters. minFilter and mipmapMode combine to correspond to the similarly named OpenGL minification filter of GL_minFilter_MIPMAP_mipmapMode (e.g. minFilter of <see cref="VkFilter.VK_FILTER_LINEAR"/> and mipmapMode of <see cref="VkSamplerMipmapMode.VK_SAMPLER_MIPMAP_MODE_NEAREST"/> correspond to GL_LINEAR_MIPMAP_NEAREST).</para>
/// <para>There are no Vulkan filter modes that directly correspond to OpenGL minification filters of GL_LINEAR or GL_NEAREST, but they can be emulated using <see cref="VkSamplerMipmapMode.VK_SAMPLER_MIPMAP_MODE_NEAREST"/>, minLod = 0, and maxLod = 0.25, and using minFilter = VK_FILTER_LINEAR or minFilter = VK_FILTER_NEAREST, respectively.</para>
/// <para>Note that using a maxLod of zero would cause magnification to always be performed, and the magFilter to always be used. This is valid, just not an exact match for OpenGL behavior. Clamping the maximum LOD to 0.25 allows the λ value to be non-zero and minification to be performed, while still always rounding down to the base level. If the minFilter and magFilter are equal, then using a maxLod of zero also works.</para>
/// <para>The maximum number of sampler objects which can be simultaneously created on a device is implementation-dependent and specified by the maxSamplerAllocationCount member of the <see cref="VkPhysicalDeviceLimits"/> structure.</para>
/// <remarks>Note: For historical reasons, if maxSamplerAllocationCount is exceeded, some implementations may return <see cref="VkResult.VK_ERROR_TOO_MANY_OBJECTS"/>. Exceeding this limit will result in undefined behavior, and an application should not rely on the use of the returned error code in order to identify when the limit is reached.</remarks>
/// <para>Since <see cref="VkSampler"/> is a non-dispatchable handle type, implementations may return the same handle for sampler state vectors that are identical. In such cases, all such objects would only count once against the maxSamplerAllocationCount limit.</para>
/// </summary>
public unsafe struct VkSamplerCreateInfo
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask of <see cref="VkSamplerCreateFlagBits"/> describing additional parameters of the sampler.
    /// </summary>
    public VkSamplerCreateFlags flags;

    /// <summary>
    /// A <see cref="VkFilter"/> value specifying the magnification filter to apply to lookups.
    /// </summary>
    public VkFilter magFilter;

    /// <summary>
    /// A <see cref="VkFilter"/> value specifying the minification filter to apply to lookups.
    /// </summary>
    public VkFilter minFilter;

    /// <summary>
    /// A <see cref="VkSamplerMipmapMode"/> value specifying the mipmap filter to apply to lookups.
    /// </summary>
    public VkSamplerMipmapMode mipmapMode;

    /// <summary>
    /// A <see cref="VkSamplerAddressMode"/> value specifying the addressing mode for U coordinates outside [0,1).
    /// </summary>
    public VkSamplerAddressMode addressModeU;

    /// <summary>
    /// A <see cref="VkSamplerAddressMode"/> value specifying the addressing mode for V coordinates outside [0,1).
    /// </summary>
    public VkSamplerAddressMode addressModeV;

    /// <summary>
    /// A <see cref="VkSamplerAddressMode"/> value specifying the addressing mode for W coordinates outside [0,1).
    /// </summary>
    public VkSamplerAddressMode addressModeW;

    /// <summary>
    /// The bias to be added to mipmap LOD calculation and bias provided by image sampling functions in SPIR-V, as described in the LOD Operation section.
    /// </summary>
    public float mipLodBias;

    /// <summary>
    /// Is true to enable anisotropic filtering, as described in the Texel Anisotropic Filtering section, or false otherwise.
    /// </summary>
    public VkBool32 anisotropyEnable;

    /// <summary>
    /// The anisotropy value clamp used by the sampler when anisotropyEnable is true. If anisotropyEnable is false, maxAnisotropy is ignored.
    /// </summary>
    public float maxAnisotropy;

    /// <summary>
    /// <para>Is true to enable comparison against a reference value during lookups, or false otherwise.</para>
    /// <remarks>Note: Some implementations will default to shader state if this member does not match.</remarks>
    /// </summary>
    public VkBool32 compareEnable;

    /// <summary>
    /// A VkCompareOp value specifying the comparison operator to apply to fetched data before filtering as described in the Depth Compare Operation section.
    /// </summary>
    public VkCompareOp compareOp;

    /// <summary>
    /// Used to clamp the minimum of the computed LOD value.
    /// </summary>
    public float minLod;

    /// <summary>
    /// Used to clamp the maximum of the computed LOD value. To avoid clamping the maximum value, set maxLod to the constant <see cref="Vk.VK_LOD_CLAMP_NONE"/>.
    /// </summary>
    public float maxLod;

    /// <summary>
    /// A <see cref="VkBorderColor"/> value specifying the predefined border color to use.
    /// </summary>
    public VkBorderColor borderColor;

    /// <summary>
    /// <para>Controls whether to use unnormalized or normalized texel coordinates to address texels of the image. When set to true, the range of the image coordinates used to lookup the texel is in the range of zero to the image size in each dimension. When set to false the range of image coordinates is zero to one.</para>
    /// <para>When unnormalizedCoordinates is true, images the sampler is used with in the shader have the following requirements:</para>
    /// <list type="bullet">
    /// <item>The viewType must be either <see cref="VkImageViewType.VK_IMAGE_VIEW_TYPE_1D"/> or <see cref="VkImageViewType.VK_IMAGE_VIEW_TYPE_2D"/>.</item>
    /// <item>The image view must have a single layer and a single mip level.</item>
    /// </list>
    /// <para>When unnormalizedCoordinates is true, image built-in functions in the shader that use the sampler have the following requirements:</para>
    /// <list type="bullet">
    /// <item>The functions must not use projection.</item>
    /// <item>The functions must not use offsets.</item>
    /// </list>
    /// </summary>
    public VkBool32 unnormalizedCoordinates;

    public VkSamplerCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO;
}
