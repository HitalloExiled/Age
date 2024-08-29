using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandPoolCreateInfo.html">VkCommandPoolCreateInfo</see>
/// </summary>
public unsafe struct VkCommandPoolCreateInfo
{
    public readonly VkStructureType SType;

    public void*                    PNext;
    public VkCommandPoolCreateFlags Flags;
    public uint                     QueueFamilyIndex;

    public VkCommandPoolCreateInfo() =>
        this.SType = VkStructureType.CommandPoolCreateInfo;
}
