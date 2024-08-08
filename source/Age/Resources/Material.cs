using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using Age.Storage;

namespace Age.Resources;

public class Material
{
    internal Pipeline Pipeline { get; }

    public Texture Diffuse { get; set; } = Texture.Default;

    public Material() =>
        this.Pipeline = ShaderStorage.Singleton.GetShaderPipeline(nameof(GeometryShader));
}
