using Age.Rendering.Resources;

namespace Age.Rendering.Interfaces;

public interface IShaderStorage : IDisposable
{
    Pipeline GetShaderPipeline(string name);
}
