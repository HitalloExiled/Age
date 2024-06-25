using Age.Rendering.Resources;
using Age.Rendering.Shaders;

namespace Age.Rendering.Scene.Resources;

public class Material
{
    internal Pipeline Pipeline { get; }

    public Texture Diffuse { get; set; } = Texture.Default;

    public Material() =>
        this.Pipeline = Container.Singleton.ShaderStorage.GetShaderPipeline(nameof(GeometryShader));
}
