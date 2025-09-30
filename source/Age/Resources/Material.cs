using Age.Core;
using Age.Rendering.Resources;
using Age.Shaders;
using Age.Storage;

namespace Age.Resources;

public sealed class Material : Disposable
{
    private Shader? shader;

    public Texture2D Diffuse { get; set; } = Texture2D.Default;

    protected override void OnDisposed(bool disposing)
    {
        if (this.Diffuse != Texture2D.Default)
        {
            this.Diffuse.Dispose();
        }
    }

    internal Shader GetShader(RenderTarget renderTarget) =>
        this.shader ??= ShaderStorage.Singleton.GetShader(nameof(GeometryShader), renderTarget.RenderPass);
}
