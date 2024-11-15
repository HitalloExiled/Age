using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using Age.Storage;

namespace Age.Resources;

public sealed class Material
{
    internal Shader Shader { get; }

    public Texture Diffuse { get; set; } = Texture.Default;

    public Material() =>
        this.Shader = ShaderStorage.Singleton.GetShader(nameof(GeometryShader));
}
