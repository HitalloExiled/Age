using Age.Core;
using Age.Rendering.Resources;
using Age.Shaders;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using Age.Graphs;
using Age.Passes;
using Age.Core.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Age.Storage;

public sealed class ShaderStorage : Disposable
{
    private readonly VulkanRenderer             renderer;
    private readonly Dictionary<string, Shader> shaders = [];

    [AllowNull]
    public static ShaderStorage Singleton { get; private set; }

    public ShaderStorage(VulkanRenderer renderer)
    {
        SingletonViolationException.ThrowIfNoSingleton(Singleton);

        Singleton = this;

        this.renderer = renderer;
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            foreach (var shader in this.shaders.Values)
            {
                this.renderer.DeferredDispose(shader);
            }
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

                        this.shaders[name] = shader = new Geometry3DShader(pass.Viewport!.RenderTarget!.RenderPass, this.renderer.MaxUsableSampleCount);

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
                    this.shaders[name] = shader = new Geometry3DShader(renderPass, SampleCount.N1);

                    break;
            }
        }

        return shader ?? throw new InvalidOperationException($"Shader {name} not found");
    }
}
