using Age.Numerics;

namespace Age.Platforms.Display;

public ref struct CursorImage(ReadOnlySpan<uint> pixels, Size<uint> size)
{
    public ReadOnlySpan<uint> Pixels = pixels;
    public Size<uint>         Size   = size;
}
