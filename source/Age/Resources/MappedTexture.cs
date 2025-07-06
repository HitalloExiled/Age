using Age.Numerics;

namespace Age.Resources;

public record struct TextureMap(Texture2D Texture, UVRect UV)
{
    public static TextureMap Default => new(Texture2D.Empty, UVRect.Normalized);

    public readonly bool IsDefault => this == Default;
}
