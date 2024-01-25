namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDeviceQueueCreateInfo.html">VkDeviceQueueCreateInfo</see>
/// </summary>
public unsafe struct VkDeviceQueueCreateInfo
{
    public readonly VkStructureType sType;

    public void*                    pNext;
    public VkDeviceQueueCreateFlags flags;
    public uint                     queueFamilyIndex;
    public uint                     queueCount;
    public float*                   pQueuePriorities;

    public VkDeviceQueueCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO;
}
