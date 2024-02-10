using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandBufferAllocateInfo.html">VkCommandBufferAllocateInfo</see>
/// </summary>
public unsafe struct VkCommandBufferAllocateInfo
{
    public readonly VkStructureType SType;

    public void*                     PNext;
    public VkHandle<VkCommandPool>   CommandPool;
    public VkCommandBufferLevel Level;
    public uint                      CommandBufferCount;

    public VkCommandBufferAllocateInfo() =>
        this.SType = VkStructureType.CommandBufferAllocateInfo;
}
