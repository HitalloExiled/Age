namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFenceCreateFlagBits.html">VkFenceCreateFlagBits</see>
/// </summary>
[Flags]
public enum FenceCreateFlags
{
    Signaled = 0x00000001,
}
