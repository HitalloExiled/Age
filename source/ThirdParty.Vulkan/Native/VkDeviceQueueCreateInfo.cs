using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDeviceQueueCreateInfo.html">VkDeviceQueueCreateInfo</see>
/// </summary>
public unsafe struct VkDeviceQueueCreateInfo
{
    public readonly VkStructureType SType;

    public void*                    PNext;
    public VkDeviceQueueCreateFlags Flags;
    public uint                     QueueFamilyIndex;
    public uint                     QueueCount;
    public float*                   PQueuePriorities;

    public VkDeviceQueueCreateInfo() =>
        this.SType = VkStructureType.DeviceQueueCreateInfo;
}
