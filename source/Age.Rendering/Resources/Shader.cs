using Age.Rendering.Shaders;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public class Shader : Disposable
{
    public event Action? Changed;

    private readonly ShaderResources shaderResources;

    public string Name => this.shaderResources.Name;

    public VkPipelineBindPoint PipelineBindPoint { get; }
    public RenderPass          RenderPass        { get; }

    public required VkDescriptorSetLayout DescriptorSetLayout { get; set; }
    public required VkPipeline            Pipeline            { get; set; }
    public required VkPipelineLayout      PipelineLayout      { get; set; }
    public required VkDescriptorType[]    UniformBindings     { get; set; }

    internal Shader(VkPipelineBindPoint pipelineBindPoint, ShaderResources shaderResources, RenderPass renderPass)
    {
        this.PipelineBindPoint = pipelineBindPoint;
        this.shaderResources   = shaderResources;
        this.RenderPass        = renderPass;

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
}

