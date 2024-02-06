namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandBufferUsageFlagBits.html">VkCommandBufferUsageFlagBits</see>
/// </summary>
[Flags]
public enum VkCommandBufferUsageFlags
{
    OneTimeSubmit      = 0x00000001,
    RenderPassContinue = 0x00000002,
    SimultaneousUse    = 0x00000004,
}
