using Age.Rendering.Resources;

namespace Age.Rendering.Interfaces;

internal interface ITextureStorage : IDisposable
{
    Texture DefaultTexture { get; }
    Sampler DefaultSampler { get; }
}
