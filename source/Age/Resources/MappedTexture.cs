using Age.Numerics;

namespace Age.Resources;

public record struct MappedTexture(Texture2D Texture, UVRect UV)
{
    public static MappedTexture Default => new(Texture2D.Empty, UVRect.Normalized);
}
