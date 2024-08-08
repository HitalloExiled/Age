using Age.Core;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using Age.Rendering.Vulkan;
using Age.RenderPasses;
using ThirdParty.Vulkan.Enums;

namespace Age.Storage;

public class ShaderStorage : Disposable
{
    private static ShaderStorage? singleton;

    public static ShaderStorage Singleton => singleton ?? throw new NullReferenceException();


    private readonly Dictionary<string, Pipeline> pipelines = [];
    private readonly VulkanRenderer renderer;

    public ShaderStorage(VulkanRenderer renderer)
    {
        singleton = this;

        this.renderer = renderer;
    }

    public Pipeline GetShaderPipeline(string name)
    {
        if (!this.pipelines.TryGetValue(name, out var shader))
        {
            switch (name)
            {
                case nameof(GeometryShader):
                    {
                        var pass = RenderGraph.Active.GetRenderGraphPass<SceneRenderGraphPass>();

                        this.pipelines[name] = shader = renderer.CreatePipelineAndWatch<GeometryShader, GeometryShader.Vertex, GeometryShader.PushConstant>(new() { RasterizationSamples = renderer.MaxUsableSampleCount, FrontFace = VkFrontFace.CounterClockwise }, pass.RenderPass);

                        break;
                    }
            }
        }

        return shader ?? throw new InvalidOperationException($"Shader {name} not found");
    }

    protected override void OnDispose() =>
        renderer.DeferredDispose(this.pipelines.Values);
}
