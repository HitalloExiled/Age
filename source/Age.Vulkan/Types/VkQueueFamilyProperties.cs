using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure providing information about a queue family</para>
/// <para>The value returned in minImageTransferGranularity has a unit of compressed texel blocks for images having a block-compressed format, and a unit of texels otherwise.</para>
/// <para>Possible values of minImageTransferGranularity are:</para>
/// <list type="bullet">
/// <item>
/// (0,0,0) specifies that only whole mip levels must be transferred using the image transfer operations on the corresponding queues. In this case, the following restrictions apply to all offset and extent parameters of image transfer operations:
/// <list type="number">
/// <item>The x, y, and z members of a <see cref="VkOffset3D"/> parameter must always be zero.</item>
/// <item>The width, height, and depth members of a <see cref="VkExtent3D"/> parameter must always match the width, height, and depth of the image subresource corresponding to the parameter, respectively.</item>
/// </list>
/// </item>
/// <item>
/// (Ax, Ay, Az) where Ax, Ay, and Az are all integer powers of two. In this case the following restrictions apply to all image transfer operations:
/// <list type="number">
/// <item>x, y, and z of a VkOffset3D parameter must be integer multiples of Ax, Ay, and Az, respectively.</item>
/// <item>width of a <see cref="VkExtent3D"/> parameter must be an integer multiple of Ax, or else x + width must equal the width of the image subresource corresponding to the parameter.</item>
/// <item>height of a <see cref="VkExtent3D"/> parameter must be an integer multiple of Ay, or else y + height must equal the height of the image subresource corresponding to the parameter.</item>
/// <item>depth of a <see cref="VkExtent3D"/> parameter must be an integer multiple of Az, or else z + depth must equal the depth of the image subresource corresponding to the parameter.</item>
/// <item>If the format of the image corresponding to the parameters is one of the block-compressed formats then for the purposes of the above calculations the granularity must be scaled up by the compressed texel block dimensions.</item>
/// </list>
/// </item>
/// </list>
/// <para>Queues supporting graphics and/or compute operations must report (1,1,1) in minImageTransferGranularity, meaning that there are no additional restrictions on the granularity of image transfer operations for these queues. Other queues supporting image transfer operations are only required to support whole mip level transfers, thus minImageTransferGranularity for queues belonging to such queue families may be (0,0,0).</para>
/// <para>The <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-device">Device Memory</see> section describes memory properties queried from the physical device.</para>
/// <para>For physical device feature queries see the Features chapter.</para>
/// </summary>
public struct VkQueueFamilyProperties
{
    /// <summary>
    /// A bitmask of <see cref="VkQueueFlagBits"/> indicating capabilities of the queues in this queue family.
    /// </summary>
    public VkQueueFlags queueFlags;

    /// <summary>
    /// The unsigned integer count of queues in this queue family. Each queue family must support at least one queue.
    /// </summary>
    public uint queueCount;

    /// <summary>
    /// The unsigned integer count of meaningful bits in the timestamps written via <see cref="Vk.CmdWriteTimestamp2"/> or <see cref="Vk.CmdWriteTimestamp"/>. The valid range for the count is 36 to 64 bits, or a value of 0, indicating no support for timestamps. Bits outside the valid range are guaranteed to be zeros.
    /// </summary>
    public uint timestampValidBits;

    /// <summary>
    /// The minimum granularity supported for image transfer operations on the queues in this queue family.
    /// </summary>
    public VkExtent3D minImageTransferGranularity;
}
