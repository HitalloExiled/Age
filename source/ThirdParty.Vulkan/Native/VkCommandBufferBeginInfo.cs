namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandBufferBeginInfo.html">VkCommandBufferBeginInfo</see>
/// </summary>
public unsafe struct VkCommandBufferBeginInfo
{
    public readonly VkStructureType sType;

    public void*                           pNext;
    public VkCommandBufferUsageFlags       flags;
    public VkCommandBufferInheritanceInfo* pInheritanceInfo;

    public VkCommandBufferBeginInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO;
}
