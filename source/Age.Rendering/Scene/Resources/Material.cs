using Age.Rendering.Resources;
using Age.Rendering.Shaders;

namespace Age.Rendering.Scene.Resources;

public class Material
{
    internal Shader Shader { get; }

    public Texture Diffuse { get; set; } = Texture.Default;

    public Material() =>
        this.Shader = Container.Singleton.ShaderStorage.GetShader(nameof(GeometryShader));
}
