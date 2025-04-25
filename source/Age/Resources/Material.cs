using Age.Rendering.Resources;
using Age.Shaders;
using Age.Storage;

namespace Age.Resources;

public sealed class Material
{
    internal Shader Shader { get; }

    public Texture2D Diffuse { get; set; } = Texture2D.Default;

    public Material() =>
        this.Shader = ShaderStorage.Singleton.GetShader(nameof(GeometryShader));
}
