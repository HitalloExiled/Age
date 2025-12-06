using System.Diagnostics.CodeAnalysis;
using Age.Core;
using Age.Core.Exceptions;
using Age.Core.Extensions;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age.Cache;

public sealed class ShaderCache : Disposable
{
    private readonly Dictionary<int, Shader> shaders = [];
    private readonly VulkanRenderer renderer;

    [AllowNull]
    public static ShaderCache Singleton { get; private set; }

    public ShaderCache(VulkanRenderer renderer)
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

    public T Get<T>(RenderTarget renderTarget, in ShaderOptions shaderOptions = default) where T : Shader, IShaderFactory
    {
        var hashcode = HashCode.Combine(typeof(T), shaderOptions);

        ref var shader = ref this.shaders.GetValueRefOrAddDefault(hashcode, out var exists);

        if (!exists)
        {
            shader = T.Create(renderTarget.RenderPass, shaderOptions);

            shader.Disposed += () => this.shaders.Remove(hashcode);

            return (T)shader;
        }

        return (T)shader!.Share();
    }
}
