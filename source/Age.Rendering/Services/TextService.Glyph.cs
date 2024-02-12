using System.Diagnostics;
using Age.Rendering.Resources;

namespace Age.Rendering.Services;

public partial class TextService
{
    [DebuggerDisplay("Character: {Character}, Size: {Size}")]
    private record Glyph
    {
        public required char    Character { get; init; }
        public required Texture Texture   { get; init; }
    }
}
