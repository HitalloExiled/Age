using Age.Core;
using Age.Rendering.Resources;
using Age.Shaders;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using Age.Graphs;
using Age.Passes;

namespace Age.Storage;

public class ShaderStorage : Disposable
{
    private static ShaderStorage? singleton;

    private readonly VulkanRenderer             renderer;
    private readonly Dictionary<string, Shader> shaders = [];

    public static ShaderStorage Singleton => singleton ?? throw new NullReferenceException();

    public ShaderStorage(VulkanRenderer renderer)
    {
        singleton = this;

        this.renderer = renderer;
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.renderer.DeferredDispose(this.shaders.Values);
        }
    }

    public Shader GetShader(string name, RenderGraph renderGraph)
    {
        if (!this.shaders.TryGetValue(name, out var shader))
        {
            switch (name)
            {
                case nameof(Geometry3DShader):
                    {
                        var pass = renderGraph.GetNode<Scene3DPass>();

                        this.shaders[name] = shader = new Geometry3DShader(pass.Viewport!.RenderTarget!.RenderPass, this.renderer.MaxUsableSampleCount, true);

                        break;
                    }
            }
        }

        return shader ?? throw new InvalidOperationException($"Shader {name} not found");
    }

    public Shader GetShader(string name, VkRenderPass renderPass)
    {
        if (!this.shaders.TryGetValue(name, out var shader))
        {
            switch (name)
            {
                case nameof(Geometry3DShader):
                    this.shaders[name] = shader = new Geometry3DShader(renderPass, SampleCount.N1, true);

                    break;
            }
        }

        return shader ?? throw new InvalidOperationException($"Shader {name} not found");
    }
}
