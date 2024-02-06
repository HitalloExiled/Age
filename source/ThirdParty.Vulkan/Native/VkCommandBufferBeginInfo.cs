using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandBufferBeginInfo.html">VkCommandBufferBeginInfo</see>
/// </summary>
public unsafe struct VkCommandBufferBeginInfo
{
    public readonly VkStructureType SType;

    public void*                           PNext;
    public VkCommandBufferUsageFlags       Flags;
    public VkCommandBufferInheritanceInfo* PInheritanceInfo;

    public VkCommandBufferBeginInfo() =>
        this.SType = VkStructureType.CommandBufferBeginInfo;
}
