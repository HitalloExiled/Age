using Age.Resources;

namespace Age.Storage;

internal partial class TextStorage
{
    public record Glyph
    {
        public required TextureMap TextureMap { get; init; }
        public required char       Character  { get; init; }

        public override string ToString() =>
            $"Character: {this.Character}, TextureMap: {this.TextureMap}";
    }
}
