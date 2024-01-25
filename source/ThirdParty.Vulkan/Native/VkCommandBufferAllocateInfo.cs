namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandBufferAllocateInfo.html">VkCommandBufferAllocateInfo</see>
/// </summary>
public unsafe struct VkCommandBufferAllocateInfo
{
    public readonly VkStructureType sType;

    public void*                pNext;
    public VkCommandPool        commandPool;
    public VkCommandBufferLevel level;
    public uint                 commandBufferCount;

    public VkCommandBufferAllocateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO;
}
