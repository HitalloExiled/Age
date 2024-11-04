using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Vulkan;
using SkiaSharp;

namespace Age.Services;

internal partial class TextStorage : IDisposable
{
    private static TextStorage? singleton;

    public static TextStorage Singleton => singleton ?? throw new NullReferenceException();

    private readonly Dictionary<int, TextureAtlas> atlases = [];
    private readonly Dictionary<int, Glyph>        glyphs = [];
    private readonly VulkanRenderer                renderer;

    private bool disposed;

    public TextStorage(VulkanRenderer renderer)
    {
        if (singleton != null)
        {
            throw new InvalidOperationException($"Only one single instace of {nameof(TextStorage)} is allowed");
        }

        singleton = this;

        this.renderer = renderer;
    }

    public Glyph DrawGlyph(TextureAtlas atlas, char character, string fontFamily, ushort fontSize, in SKRect bounds, SKPaint paint)
    {
        const ushort PADDING = 2;

        var hashcode = character.GetHashCode() ^ fontFamily.GetHashCode() ^ fontSize.GetHashCode();

        if (!this.glyphs.TryGetValue(hashcode, out var glyph))
        {
            var charString = character.ToString();

            using var bitmap = new SKBitmap(
                (int)bounds.Width + PADDING * 2,
                (int)bounds.Height + PADDING * 2
            );

            using var canvas = new SKCanvas(bitmap);

            canvas.DrawText(charString, PADDING + -bounds.Location.X, PADDING + -bounds.Location.Y, paint);

            var position = atlas.Pack(bitmap.Pixels.AsSpan().Cast<SKColor, uint>(), new((uint)bitmap.Width, (uint)bitmap.Height));

            this.glyphs[hashcode] = glyph = new()
            {
                Atlas     = atlas,
                Character = character,
                Position  = position + PADDING,
                Size      = new((uint)bounds.Width, (uint)bounds.Height),
            };
        }

        return glyph;
    }

    public TextureAtlas GetAtlas(string familyName, uint fontSize)
    {
        var hashcode = familyName.GetHashCode() ^ fontSize.GetHashCode();

        if (!this.atlases.TryGetValue(hashcode, out var atlas))
        {
            var axisSize = uint.Max(fontSize * 8, 256);
            var size     = new Size<uint>(axisSize, axisSize);

            this.atlases[hashcode] = atlas = new(size, ColorMode.Grayscale);
        }

        return atlas;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                foreach (var atlas in this.atlases.Values)
                {
                    this.renderer.DeferredDispose(atlas);
                }
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
