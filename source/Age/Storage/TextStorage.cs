using Age.Core;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Vulkan;
using SkiaSharp;

namespace Age.Services;

internal partial class TextStorage : Disposable
{
    private static TextStorage? singleton;

    public static TextStorage Singleton => singleton ?? throw new NullReferenceException();

    private readonly Dictionary<int, TextureAtlas> atlases = [];
    private readonly Dictionary<int, Glyph>        glyphs = [];
    private readonly Dictionary<int, SKPaint>      paints = [];
    private readonly VulkanRenderer                renderer;

    public TextStorage(VulkanRenderer renderer)
    {
        if (singleton != null)
        {
            throw new InvalidOperationException($"Only one single instace of {nameof(TextStorage)} is allowed");
        }

        singleton = this;

        this.renderer = renderer;
    }

    protected override void Disposed(bool disposing)
    {
        if (disposing)
        {
            foreach (var atlas in this.atlases.Values)
            {
                this.renderer.DeferredDispose(atlas);
            }

            foreach (var paint in this.paints.Values)
            {
                paint.Dispose();
            }

            this.atlases.Clear();
            this.paints.Clear();
        }
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

            var position = atlas.Pack(bitmap.GetPixelSpan().Cast<byte, uint>(), new((uint)bitmap.Width, (uint)bitmap.Height));

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

        ref var atlas = ref this.atlases.GetValueRefOrAddDefault(hashcode, out var exists);

        if (!exists)
        {
            var axisSize = uint.Max(fontSize * 8, 256);
            var size     = new Size<uint>(axisSize, axisSize);

            atlas = new(size, ColorMode.Grayscale);
        }

        return atlas!;
    }

    public SKPaint GetPaint(string fontFamily, float fontSize, int fontWeight)
    {
        var hashcode = HashCode.Combine(fontFamily, fontSize, fontWeight);

        ref var paint = ref this.paints.GetValueRefOrAddDefault(hashcode, out var exists);

        if (!exists)
        {
            paint = new SKPaint
            {
                Color        = SKColors.Black,
                IsAntialias  = true,
                TextAlign    = SKTextAlign.Left,
                TextSize     = fontSize,
                Typeface     = SKTypeface.FromFamilyName(fontFamily, (SKFontStyleWeight)fontWeight, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
                SubpixelText = false,
            };
        }

        return paint!;
    }
}
