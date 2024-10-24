using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkClearAttachment.html">VkClearAttachment</see>
/// </summary>
public struct VkClearAttachment
{
    public VkImageAspectFlags AspectMask;
    public uint               ColorAttachment;
    public VkClearValue       ClearValue;
}
