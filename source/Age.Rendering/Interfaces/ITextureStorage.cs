using Age.Numerics;
using Age.Rendering.Enums;
using Age.Rendering.Resources;

namespace Age.Rendering.Interfaces;

internal interface ITextureStorage : IDisposable
{
    Texture DefaultTexture { get; }
    Sampler DefaultSampler { get; }

    Texture CreateTexture(Size<uint> size, ColorMode colorMode, TextureType textureType);
    void FreeTexture(Texture texture);
    UniformSet GetUniformSet(Shader shader, Texture texture, Sampler sampler);
}
