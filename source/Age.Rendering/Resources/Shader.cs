using Age.Core;
using Age.Rendering.Shaders;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public record Shader : Disposable
{
    public event Action? Changed;

    private readonly ShaderResources shaderResources;

    public string                Name                => this.shaderResources.Name;
    public VkDescriptorSetLayout DescriptorSetLayout { get; private set; }
    public VkPipeline            Pipeline            { get; private set; }
    public VkPipelineBindPoint   PipelineBindPoint   { get; private set; }
    public VkPipelineLayout      PipelineLayout      { get; private set; }
    public RenderPass            RenderPass          { get; set; }

    public Shader(ShaderResources shaderResources, VkPipelineBindPoint pipelineBindPoint, VkPipeline pipeline, VkPipelineLayout pipelineLayout, VkDescriptorSetLayout descriptorSetLayout, RenderPass renderPass)
    {
        this.shaderResources = shaderResources;

        this.PipelineBindPoint   = pipelineBindPoint;
        this.Pipeline            = pipeline;
        this.PipelineLayout      = pipelineLayout;
        this.DescriptorSetLayout = descriptorSetLayout;
        this.RenderPass          = renderPass;

        shaderResources.Changed += this.OnResourcesChanged;
    }

    private void OnResourcesChanged() =>
        Changed?.Invoke();

    protected override void OnDispose()
    {
        this.Pipeline.Dispose();
        this.PipelineLayout.Dispose();
        this.DescriptorSetLayout.Dispose();
        this.RenderPass.Dispose();
        this.shaderResources.Dispose();
    }

    public IDisposable Update(VkPipeline pipeline, VkPipelineLayout pipelineLayout, VkDescriptorSetLayout descriptorSetLayout)
    {
        var disposables = new Disposables(this.Pipeline, this.PipelineLayout, this.DescriptorSetLayout);

        this.Pipeline            = pipeline;
        this.PipelineLayout      = pipelineLayout;
        this.DescriptorSetLayout = descriptorSetLayout;

        return disposables;
    }
}

