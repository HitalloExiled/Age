namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageCreateInfo.html">VkImageCreateInfo</see>
/// </summary>
public unsafe struct VkImageCreateInfo
{
    public readonly VkStructureType sType;

    public void*                 pNext;
    public VkImageCreateFlags    flags;
    public VkImageType           imageType;
    public VkFormat              format;
    public VkExtent3D            extent;
    public uint                  mipLevels;
    public uint                  arrayLayers;
    public VkSampleCountFlagBits samples;
    public VkImageTiling         tiling;
    public VkImageUsageFlags     usage;
    public VkSharingMode         sharingMode;
    public uint                  queueFamilyIndexCount;
    public uint*                 pQueueFamilyIndices;
    public VkImageLayout         initialLayout;

    public VkImageCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO;
}
