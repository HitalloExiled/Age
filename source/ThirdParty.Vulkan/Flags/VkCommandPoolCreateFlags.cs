namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandPoolCreateFlagBits.html">VkCommandPoolCreateFlagBits</see>
/// </summary>
[Flags]
public enum VkCommandPoolCreateFlags
{
    Transient          = 0x00000001,
    ResetCommandBuffer = 0x00000002,
    Protected          = 0x00000004,
}
