using Age.Numerics;
using Age.Storage;

namespace Age.Resources;

public record struct MappedTexture(Rendering.Resources.Texture Texture, UVRect UV)
{
    public static MappedTexture Default => new(TextureStorage.Singleton.EmptyTexture, UVRect.Normalized);
}
