using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static class PipelineTessellationState
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineTessellationStateCreateInfo.html">VkPipelineTessellationStateCreateInfo</see>
    /// </summary>
    public record CreateInfo : NativeReference<VkPipelineTessellationStateCreateInfo>
    {

    }
}
