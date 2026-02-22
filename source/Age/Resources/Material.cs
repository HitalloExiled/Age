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

        this.shader?.Dispose();
    }

    internal Shader GetShader(RenderTarget renderTarget, in ShaderOptions shaderOptions) =>
        this.shader ??= ShaderStorage.Singleton.Get<Geometry3DColorShader>(renderTarget, shaderOptions);
}
