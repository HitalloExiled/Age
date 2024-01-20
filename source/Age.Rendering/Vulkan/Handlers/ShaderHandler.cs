using Age.Vulkan.Enums;
using Age.Vulkan.Types;

namespace Age.Rendering.Vulkan.Handlers;

public record ShaderHandler
{
    public required VkDescriptorSetLayout DescriptorSetLayout { get; init; }
    public required VkPipeline            Pipeline            { get; init; }
    public required VkPipelineBindPoint   PipelineBindPoint   { get; init; }
    public required VkPipelineLayout      PipelineLayout      { get; init; }
    public required VkRenderPass          RenderPass          { get; init; }
}

