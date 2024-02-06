using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPushConstantRange.html">VkPushConstantRange</see>
/// </summary>
public struct VkPushConstantRange
{
    public VkShaderStageFlags StageFlags;
    public uint               Offset;
    public uint               Size;
}
