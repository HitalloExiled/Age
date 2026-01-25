using System.Diagnostics.CodeAnalysis;
using Age.Core;
using Age.Core.Exceptions;
using Age.Core.Extensions;
using Age.Rendering;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

using ShaderKey = (ThirdParty.Vulkan.VkRenderPass, System.Type, Age.Rendering.Resources.ShaderOptions);

namespace Age.Storage;

public sealed class ShaderStorage : Disposable
{
    private readonly Dictionary<ShaderKey, Shader> shaders = [];
    private readonly VulkanRenderer renderer;
    private readonly ShaderCompiler shaderCompiler = new(true);

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

    public TShader Get<TShader>(RenderTarget renderTarget, in ShaderOptions? shaderOptions = null) where TShader : Shader, IShaderFactory<TShader>
    {
        var key = (renderTarget.RenderPass, typeof(TShader), shaderOptions.GetValueOrDefault());

        ref var shader = ref this.shaders.GetValueRefOrAddDefault(key, out var exists);

        if (!exists)
        {
            shader = TShader.Create(renderTarget.RenderPass);

            this.shaderCompiler.CompileShader(shader, shaderOptions ?? new());

            shader.Disposed += () => this.shaders.Remove(key);

            return (TShader)shader;
        }

        return (TShader)shader!.Share();
    }
}
