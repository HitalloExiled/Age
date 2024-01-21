using System.Diagnostics;
using SkiaSharp;

namespace Age.Rendering.Services;

public partial class TextService
{
    [DebuggerDisplay("Character: {Character}, Size: {Size}")]
    private record Glyph
    {
        public required SKRect  Bounds    { get; init; }
        public required char    Character { get; init; }
        public required Texture Texture   { get; init; }
    }
}
