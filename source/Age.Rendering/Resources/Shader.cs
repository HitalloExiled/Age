using Age.Core;
using Age.Rendering.Interfaces;
using Age.Rendering.Vulkan;
using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;
using System.Diagnostics;
using Age.Core.Collections;

namespace Age.Rendering.Resources;

public interface IShaderFactory<T> where T : Shader
{
    public abstract static T Create(VkRenderPass renderPass);
}

public abstract class Shader(string filepath, VkRenderPass renderPass) : SharedDisposable<Shader>
{
    public static string ShadersPath { get; } = Path.GetFullPath(Debugger.IsAttached ? Path.Join(Directory.GetCurrentDirectory(), "source/Age/Shaders") : Path.Join(AppContext.BaseDirectory, "Shaders"));

    public event Action? Changed;

    internal abstract NativeRefArray<VkVertexInputAttributeDescription> GetAttributes();
    internal abstract VkVertexInputBindingDescription GetBindings();

    public string       Filepath   { get; } = Path.IsPathRooted(filepath) ? filepath : Path.Join(ShadersPath, filepath);
    public VkRenderPass RenderPass { get; } = renderPass;

    public abstract VkPipelineBindPoint BindPoint         { get; }
    public abstract VkFrontFace         FrontFace         { get; }
    public abstract string              Name              { get; }
    public abstract VkPrimitiveTopology PrimitiveTopology { get; }

    public VkDescriptorSetLayout? DescriptorSetLayout { get; internal set; }
    public VkPipeline?            Pipeline            { get; internal set; }
    public VkPipelineLayout?      PipelineLayout      { get; internal set; }
    public VkShaderStageFlags     PushConstantStages  { get; internal set; }
    public VkDescriptorType[]?    UniformBindings     { get; internal set; }

    [MemberNotNullWhen(true, nameof(DescriptorSetLayout), nameof(Pipeline), nameof(PipelineLayout), nameof(UniformBindings))]
    public bool IsCompiled { get; internal set; }

    internal List<string> Dependencies = [];

    internal void InvokeChanged() =>
        this.Changed?.Invoke();
}

public abstract class Shader<TVertexInput>(string file, VkRenderPass renderPass) : Shader(file, renderPass)
where TVertexInput : IVertexInput
{
    protected Shader(string file, RenderTarget renderTarget) : this(file, renderTarget.RenderPass) { }

    internal override NativeRefArray<VkVertexInputAttributeDescription> GetAttributes() => TVertexInput.GetAttributes();
    internal override VkVertexInputBindingDescription             GetBindings()   => TVertexInput.GetBindings();

    protected override void OnDisposed(bool disposing)
    {
        if (!disposing || !this.IsCompiled)
        {
            return;
        }

        VulkanRenderer.Singleton.DeferredDispose(this.Pipeline);
        VulkanRenderer.Singleton.DeferredDispose(this.PipelineLayout);
        VulkanRenderer.Singleton.DeferredDispose(this.DescriptorSetLayout);
    }
}
