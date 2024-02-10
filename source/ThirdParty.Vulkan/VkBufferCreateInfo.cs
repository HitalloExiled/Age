using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferCreateInfo.html">VkBufferCreateInfo</see>
/// </summary>
public unsafe struct VkBufferCreateInfo
{
    public readonly VkStructureType SType;

    public void*               PNext;
    public VkBufferCreateFlags Flags;
    public VkDeviceSize        Size;
    public VkBufferUsageFlags  Usage;
    public VkSharingMode       SharingMode;
    public uint                QueueFamilyIndexCount;
    public uint*               PQueueFamilyIndices;

    public VkBufferCreateInfo() =>
        this.SType = VkStructureType.BufferCreateInfo;
}
