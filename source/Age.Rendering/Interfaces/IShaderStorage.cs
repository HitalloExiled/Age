using Age.Rendering.Resources;

namespace Age.Rendering.Interfaces;

public interface IShaderStorage : IDisposable
{
    Shader GetShader(string name);
}
