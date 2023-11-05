using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Types;

namespace Age.Rendering.Vulkan.Handlers;

public record ShaderHandler
{
    public required VkDescriptorSetLayout DescriptorSetLayout { get; init; }
    public required VkPipeline            Pipeline            { get; init; }
    public required VkPipelineBindPoint   PipelineBindPoint   { get; init; }
    public required VkPipelineLayout      PipelineLayout      { get; init; }
}

