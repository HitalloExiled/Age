using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageCreateInfo.html">VkImageCreateInfo</see>
/// </summary>
public unsafe struct VkImageCreateInfo
{
    public readonly VkStructureType SType;

    public void*                 PNext;
    public VkImageCreateFlags    Flags;
    public VkImageType           ImageType;
    public VkFormat              Format;
    public VkExtent3D            Extent;
    public uint                  MipLevels;
    public uint                  ArrayLayers;
    public VkSampleCountFlags    Samples;
    public VkImageTiling         Tiling;
    public VkImageUsageFlags     Usage;
    public VkSharingMode         SharingMode;
    public uint                  QueueFamilyIndexCount;
    public uint*                 PQueueFamilyIndices;
    public VkImageLayout         InitialLayout;

    public VkImageCreateInfo() =>
        this.SType = VkStructureType.ImageCreateInfo;
}
