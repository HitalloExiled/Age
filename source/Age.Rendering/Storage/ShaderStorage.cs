using Age.Rendering.Interfaces;
using Age.Rendering.RenderPasses;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Storage;

public class ShaderStorage(VulkanRenderer renderer) : Disposable, IShaderStorage
{
    private readonly Dictionary<string, Shader> shaders = [];

    public Shader GetShader(string name)
    {
        if (!this.shaders.TryGetValue(name, out var shader))
        {
            switch (name)
            {
                case nameof(GeometryShader):
                    {
                        var renderPass = RenderGraph.Active?.GetRenderPass<SceneRenderGraphPass>() ?? throw new InvalidOperationException();

                        this.shaders[name] = shader = renderer.CreateShader<GeometryShader, GeometryShader.Vertex, GeometryShader.PushConstant>(new() { RasterizationSamples = renderer.MaxUsableSampleCount, FrontFace = VkFrontFace.CounterClockwise }, renderPass);

                        break;
                    }
            }
        }

        return shader ?? throw new InvalidOperationException($"Shader {name} not found");
    }

    protected override void OnDispose() =>
        renderer.DeferredDispose(this.shaders.Values);
}
