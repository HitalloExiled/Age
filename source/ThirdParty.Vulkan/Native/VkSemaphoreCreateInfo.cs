namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSemaphoreCreateInfo.html">VkSemaphoreCreateInfo</see>
/// </summary>
public unsafe struct VkSemaphoreCreateInfo
{
    public readonly VkStructureType SType;

    public void*                  PNext;
    public VkSemaphoreCreateFlags Flags;

    public VkSemaphoreCreateInfo() =>
        this.SType = VkStructureType.SemaphoreCreateInfo;
}
