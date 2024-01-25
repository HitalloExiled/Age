namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandPoolCreateInfo.html">VkCommandPoolCreateInfo</see>
/// </summary>
public unsafe struct VkCommandPoolCreateInfo
{
    public readonly VkStructureType sType;

    public void*                    pNext;
    public VkCommandPoolCreateFlags flags;
    public uint                     queueFamilyIndex;

    public VkCommandPoolCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO;
}
