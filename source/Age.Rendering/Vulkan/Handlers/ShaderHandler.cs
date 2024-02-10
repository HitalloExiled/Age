using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Vulkan.Handlers;

public record ShaderHandler
{
    public required VkDescriptorSetLayout DescriptorSetLayout { get; init; }
    public required VkPipeline            Pipeline            { get; init; }
    public required VkPipelineBindPoint   PipelineBindPoint   { get; init; }
    public required VkPipelineLayout      PipelineLayout      { get; init; }
    public required VkRenderPass          RenderPass          { get; init; }
}

