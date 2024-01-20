using System.Runtime.CompilerServices;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying an image blit operation
/// </summary>
public unsafe struct VkImageBlit
{
    /// <summary>
    /// The subresource to blit from.
    /// </summary>
    public VkImageSubresourceLayers srcSubresource;

    /// <summary>
    /// A pointer to an array of two <see cref="VkOffset3D"/> structures specifying the bounds of the source region within srcSubresource.
    /// </summary>
    public fixed byte srcOffsets[2 * 12];

    /// <summary>
    /// The subresource to blit into.
    /// </summary>
    public VkImageSubresourceLayers dstSubresource;

    /// <summary>
    /// A pointer to an array of two <see cref="VkOffset3D"/> structures specifying the bounds of the destination region within dstSubresource.
    /// </summary>
    public fixed byte dstOffsets[2 * 12];

    public VkOffset3D GetDstOffsets(int index)
    {
        fixed (byte* pDstOffsets = this.dstOffsets)
        {
            return Unsafe.Read<VkOffset3D>(pDstOffsets + sizeof(VkOffset3D) * index);
        }
    }

    public VkOffset3D GetSrcOffsets(int index)
    {
        fixed (byte* pSrcOffsets = this.srcOffsets)
        {
            return Unsafe.Read<VkOffset3D>(pSrcOffsets + sizeof(VkOffset3D) * index);
        }
    }

    public void SetDstOffsets(int index, in VkOffset3D value)
    {
        fixed (byte* pDstOffsets = this.dstOffsets)
        {
            Unsafe.Write(pDstOffsets + sizeof(VkOffset3D) * index, value);
        }
    }

    public void SetSrcOffsets(int index, in VkOffset3D value)
    {
        fixed (byte* pSrcOffsets = this.srcOffsets)
        {
            Unsafe.Write(pSrcOffsets + sizeof(VkOffset3D) * index, value);
        }
    }
}
