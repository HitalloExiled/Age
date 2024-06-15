using Age.Rendering.Interfaces;
using Age.Rendering.RenderPasses;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using Age.Rendering.Vulkan;

namespace Age.Rendering.Storage;

public class ShaderStorage(VulkanRenderer renderer) : Resource(renderer), IShaderStorage
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
                        var renderPass = RenderGraph.Active?.GetRenderPass<GeometryRenderGraphPass>() ?? throw new InvalidOperationException();

                        shader = this.Renderer.CreateShader<GeometryShader, GeometryShader.Vertex, GeometryShader.PushConstant>(new() { RasterizationSamples = this.Renderer.MaxUsableSampleCount, FrontFace = ThirdParty.Vulkan.Enums.VkFrontFace.CounterClockwise }, renderPass);

                        break;
                    }
            }
        }

        return shader ?? throw new InvalidOperationException($"Shader {name} not found");
    }

    protected override void OnDispose()
    {
        foreach (var shader in this.shaders.Values)
        {
            shader.Dispose();
        }
    }
}
