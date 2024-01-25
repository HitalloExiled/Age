namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPushConstantRange.html">VkPushConstantRange</see>
/// </summary>
public struct VkPushConstantRange
{
    public VkShaderStageFlags stageFlags;
    public uint               offset;
    public uint               size;
}
