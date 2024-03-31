using System.Diagnostics;
using Age.Numerics;

namespace Age.Rendering.Services;

internal partial class TextService
{
    [DebuggerDisplay("Character: {Character}, Size: {Size}, Position: {Position}")]
    private record Glyph
    {
        public required TextureAtlas Atlas     { get; init; }
        public required char         Character { get; init; }
        public required Point<uint>  Position  { get; init; }
        public required Size<uint>   Size      { get; init; }
    }
}
