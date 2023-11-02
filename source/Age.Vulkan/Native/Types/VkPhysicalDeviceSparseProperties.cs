namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying physical device sparse memory properties.
/// </summary>
public struct VkPhysicalDeviceSparseProperties
{
    /// <summary>
    /// Is true if the physical device will access all single-sample 2D sparse resources using the standard sparse image block shapes (based on image format), as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#sparsememory-sparseblockshapessingle">Standard Sparse Image Block Shapes (Single Sample)</see> table. If this property is not supported the value returned in the imageGranularity member of the VkSparseImageFormatProperties structure for single-sample 2D images is not required to match the standard sparse image block dimensions listed in the table.
    /// </summary>
    public VkBool32 residencyStandard2DBlockShape;

    /// <summary>
    /// Is true if the physical device will access all multisample 2D sparse resources using the standard sparse image block shapes (based on image format), as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#sparsememory-sparseblockshapesmsaa">Standard Sparse Image Block Shapes (MSAA)</see> table. If this property is not supported, the value returned in the imageGranularity member of the VkSparseImageFormatProperties structure for multisample 2D images is not required to match the standard sparse image block dimensions listed in the table.
    /// </summary>
    public VkBool32 residencyStandard2DMultisampleBlockShape;

    /// <summary>
    /// Is true if the physical device will access all 3D sparse resources using the standard sparse image block shapes (based on image format), as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#sparsememory-sparseblockshapessingle">Standard Sparse Image Block Shapes (Single Sample)</see> table. If this property is not supported, the value returned in the imageGranularity member of the VkSparseImageFormatProperties structure for 3D images is not required to match the standard sparse image block dimensions listed in the table.
    /// </summary>
    public VkBool32 residencyStandard3DBlockShape;

    /// <summary>
    /// Is true if images with mip level dimensions that are not integer multiples of the corresponding dimensions of the sparse image block may be placed in the mip tail. If this property is not reported, only mip levels with dimensions smaller than the imageGranularity member of the <see cref="VkSparseImageFormatProperties"/> structure will be placed in the mip tail. If this property is reported the implementation is allowed to return <see cref="VkSparseImageFormatFlagBits.VK_SPARSE_IMAGE_FORMAT_ALIGNED_MIP_SIZE_BIT"/> in the flags member of <see cref="VkSparseImageFormatProperties"/>, indicating that mip level dimensions that are not integer multiples of the corresponding dimensions of the sparse image block will be placed in the mip tail.
    /// </summary>
    public VkBool32 residencyAlignedMipSize;

    /// <summary>
    /// Specifies whether the physical device can consistently access non-resident regions of a resource. If this property is true, access to non-resident regions of resources will be guaranteed to return values as if the resource was populated with 0; writes to non-resident regions will be discarded.
    /// </summary>
    public VkBool32 residencyNonResidentStrict;
}
