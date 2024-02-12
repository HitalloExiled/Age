using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public record Shader : Disposable
{
    public required VkDescriptorSetLayout DescriptorSetLayout { get; init; }
    public required VkPipeline            Pipeline            { get; init; }
    public required VkPipelineBindPoint   PipelineBindPoint   { get; init; }
    public required VkPipelineLayout      PipelineLayout      { get; init; }
    public required VkRenderPass          RenderPass          { get; init; }

    protected override void OnDispose()
    {
        this.Pipeline.Dispose();
        this.PipelineLayout.Dispose();
        this.DescriptorSetLayout.Dispose();
        this.RenderPass.Dispose();
    }
}

