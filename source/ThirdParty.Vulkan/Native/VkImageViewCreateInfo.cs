namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageViewCreateInfo.html">VkImageViewCreateInfo</see>
/// </summary>
public unsafe struct VkImageViewCreateInfo
{
    public readonly VkStructureType sType;

    public void*                   pNext;
    public VkImageViewCreateFlags  flags;
    public VkImage                 image;
    public VkImageViewType         viewType;
    public VkFormat                format;
    public VkComponentMapping      components;
    public VkImageSubresourceRange subresourceRange;

    public VkImageViewCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO;
}
