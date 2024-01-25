namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSemaphoreCreateInfo.html">VkSemaphoreCreateInfo</see>
/// </summary>
public unsafe struct VkSemaphoreCreateInfo
{
    public readonly VkStructureType sType;

    public void*                  pNext;
    public VkSemaphoreCreateFlags flags;

    public VkSemaphoreCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO;
}
