namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferCreateInfo.html">VkBufferCreateInfo</see>
/// </summary>
public unsafe struct VkBufferCreateInfo
{
    public readonly VkStructureType sType;

    public void*               pNext;
    public VkBufferCreateFlags flags;
    public VkDeviceSize        size;
    public VkBufferUsageFlags  usage;
    public VkSharingMode       sharingMode;
    public uint                queueFamilyIndexCount;
    public uint*               pQueueFamilyIndices;

    public VkBufferCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO;
}
