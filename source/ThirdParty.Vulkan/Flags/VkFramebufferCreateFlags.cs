namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFramebufferCreateFlagBits.html">VkFramebufferCreateFlagBits</see>
/// </summary>
[Flags]
public enum VkFramebufferCreateFlags
{
    Imageless    = 0x00000001,
    ImagelessKHR = Imageless,
}
