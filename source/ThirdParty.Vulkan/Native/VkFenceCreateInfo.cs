namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFenceCreateInfo.html">VkFenceCreateInfo</see>
/// </summary>
public unsafe struct VkFenceCreateInfo
{
    public readonly VkStructureType sType;

    public void*              pNext;
    public VkFenceCreateFlags flags;

    public VkFenceCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_FENCE_CREATE_INFO;
}
