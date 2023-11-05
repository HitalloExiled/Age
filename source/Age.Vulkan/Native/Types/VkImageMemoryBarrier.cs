using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying the parameters of an image memory barrier.</para>
/// <para>The first <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-access-scopes">access scope</see> is limited to access to memory through the specified image subresource range, via access types in the <see href="e source access mask sp">source access mask</see> specified by srcAccessMask. If srcAccessMask includes <see cref="VkAccessFlagBits.VK_ACCESS_HOST_WRITE_BIT"/>, memory writes performed by that access type are also made visible, as that access type is not performed through a resource.</para>
/// <para>The second <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-access-scopes">access scope</see> is limited to access to memory through the specified image subresource range, via access types in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-access-masks">destination access mask</see> specified by dstAccessMask. If dstAccessMask includes <see cref="VkAccessFlagBits.VK_ACCESS_HOST_WRITE_BIT"/> or <see cref="VkAccessFlagBits.VK_ACCESS_HOST_READ_BIT"/>, available memory writes are also made visible to accesses of those types, as those access types are not performed through a resource.</para>
/// <para>If srcQueueFamilyIndex is not equal to dstQueueFamilyIndex, and srcQueueFamilyIndex is equal to the current queue family, then the memory barrier defines a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-queue-transfers-release">queue family release operation</see> for the specified image subresource range, and the second access scope includes no access, as if dstAccessMask was 0.</para>
/// <para>If dstQueueFamilyIndex is not equal to srcQueueFamilyIndex, and dstQueueFamilyIndex is equal to the current queue family, then the memory barrier defines a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-queue-transfers-acquire">queue family acquire operation</see> for the specified image subresource range, and the first access scope includes no access, as if srcAccessMask was 0.</para>
/// <para>If the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-synchronization2">synchronization2</see> feature is not enabled or oldLayout is not equal to newLayout, oldLayout and newLayout define an <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-image-layout-transitions">image layout transition</see> for the specified image subresource range.</para>
/// <remarks>Note: If the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-synchronization2">synchronization2</see> feature is enabled, when the old and new layout are equal, the layout values are ignored - data is preserved no matter what values are specified, or what layout the image is currently in.</remarks>
/// <para>If image has a multi-planar format and the image is disjoint, then including <see cref="VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT"/> in the aspectMask member of subresourceRange is equivalent to including <see cref="VkImageAspectFlagBits.VK_IMAGE_ASPECT_PLANE_0_BIT"/>, <see cref="VkImageAspectFlagBits.VK_IMAGE_ASPECT_PLANE_1_BIT"/>, and (for three-plane formats only) <see cref="VkImageAspectFlagBits.VK_IMAGE_ASPECT_PLANE_2_BIT"/>.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkImageMemoryBarrier
{
    /// sType is a VkStructureType value identifying this structure.
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask of <see cref="VkAccessFlagBits"/> specifying a source access mask.
    /// </summary>
    public VkAccessFlags srcAccessMask;

    /// <summary>
    /// A bitmask of <see cref="VkAccessFlagBits"/> specifying a destination access mask.
    /// </summary>
    public VkAccessFlags dstAccessMask;

    /// <summary>
    /// The old layout in an image layout transition.
    /// </summary>
    public VkImageLayout oldLayout;

    /// <summary>
    /// The new layout in an image layout transition.
    /// </summary>
    public VkImageLayout newLayout;

    /// <summary>
    /// The source queue family for a queue family ownership transfer.
    /// </summary>
    public uint srcQueueFamilyIndex;

    /// <summary>
    /// The destination queue family for a queue family ownership transfer.
    /// </summary>
    public uint dstQueueFamilyIndex;

    /// <summary>
    /// A handle to the image affected by this barrier.
    /// </summary>
    public VkImage image;

    /// <summary>
    /// Describes the image subresource range within image that is affected by this barrier.
    /// </summary>
    public VkImageSubresourceRange subresourceRange;

    public VkImageMemoryBarrier() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER;
}
