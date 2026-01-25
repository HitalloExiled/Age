using System.Diagnostics;
using System.Runtime.CompilerServices;
using SkiaSharp;

namespace Age.Storage;

internal sealed partial class TextStorage
{
    [InlineArray(2)]
    private struct Chars
    {
#pragma warning disable RCS1169, RCS1213 // Make field read-only
        private char element;
#pragma warning restore RCS1169, RCS1213 // Make field read-only

        public Chars(ReadOnlySpan<char> chars)
        {
            Debug.Assert(chars.Length <= 2);

            chars.CopyTo(this);
        }
    }

    private readonly struct GlyphKey(ReadOnlySpan<char> chars, SKFont sKFont)
    {
        public readonly Chars  Chars  = new(chars);
        public readonly SKFont SKFont = sKFont;
    }
}
