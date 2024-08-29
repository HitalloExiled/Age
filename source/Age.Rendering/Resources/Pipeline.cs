using Age.Rendering.Shaders;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public class Pipeline : Resource
{
    public event Action? Changed;

    private readonly Shader shader;

    public string Name => this.shader.Name;

    public VkPipelineBindPoint BindPoint  { get; }
    public RenderPass          RenderPass { get; }

    public VkPipeline Value { get; internal set; }

    public required VkDescriptorSetLayout DescriptorSetLayout { get; set; }
    public required VkPipelineLayout      Layout              { get; set; }
    public required VkDescriptorType[]    UniformBindings     { get; set; }

    internal Pipeline(VkPipeline pipeline, VkPipelineBindPoint pipelineBindPoint, Shader shader, RenderPass renderPass)
    {
        this.Value      = pipeline;
        this.BindPoint  = pipelineBindPoint;
        this.shader     = shader;
        this.RenderPass = renderPass;

        shader.Changed += this.OnShaderChanged;
    }

    private void OnShaderChanged() =>
        Changed?.Invoke();

    protected override void OnDispose()
    {
        this.Value.Dispose();
        this.Layout.Dispose();
        this.DescriptorSetLayout.Dispose();
        this.shader.Dispose();
    }
}

