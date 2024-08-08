using Age.Rendering.Interfaces;
using Age.Rendering.RenderPasses;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Storage;

public class ShaderStorage(VulkanRenderer renderer) : Disposable, IShaderStorage
{
    private readonly Dictionary<string, Pipeline> pipelines = [];

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
