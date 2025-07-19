using Age.Core;
using Age.Rendering.Resources;
using Age.Shaders;
using Age.Rendering.Vulkan;
using Age.RenderPasses;

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

    public Shader GetShader(string name)
    {
        if (!this.shaders.TryGetValue(name, out var shader))
        {
            switch (name)
            {
                case nameof(GeometryShader):
                    {
                        var pass = RenderGraph.Active.GetRenderGraphPass<SceneRenderGraphPass>();

                        this.shaders[name] = shader = new GeometryShader(pass.RenderPass, this.renderer.MaxUsableSampleCount, true);

                        break;
                    }
            }
        }

        return shader ?? throw new InvalidOperationException($"Shader {name} not found");
    }
}
