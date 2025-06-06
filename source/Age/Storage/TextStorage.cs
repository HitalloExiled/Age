using Age.Core;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Vulkan;
using Age.Resources;
using SkiaSharp;

namespace Age.Services;

internal partial class TextStorage : Disposable
{
    private static TextStorage? singleton;

    public static TextStorage Singleton => singleton ?? throw new NullReferenceException();

    private readonly Dictionary<int, TextureAtlas>               atlases    = [];
    private readonly Dictionary<string, Dictionary<string, int>> codepoints = [];
    private readonly Dictionary<int, SKFont>                     fonts      = [];
    private readonly Dictionary<int, Glyph>                      glyphs     = [];
    private readonly SKPaint                                     paint;
    private readonly VulkanRenderer                              renderer;

    public TextStorage(VulkanRenderer renderer)
    {
        if (singleton != null)
        {
            throw new InvalidOperationException($"Only one single instace of {nameof(TextStorage)} is allowed");
        }

        singleton = this;

        this.renderer = renderer;
        this.paint    = new()
        {
            Color       = SKColors.Black,
            IsAntialias = true,
        };
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            foreach (var atlas in this.atlases.Values)
            {
                this.renderer.DeferredDispose(atlas);
            }

            foreach (var font in this.fonts.Values)
            {
                font.Dispose();
            }

            this.paint.Dispose();

            this.atlases.Clear();
            this.fonts.Clear();
        }
    }

    public Glyph DrawGlyph(SKFont font, TextureAtlas atlas, char character, in SKRect bounds)
    {
        const ushort PADDING = 2;

        var hashcode = character.GetHashCode() ^ font.GetHashCode() ^ font.Size.GetHashCode();

        if (!this.glyphs.TryGetValue(hashcode, out var glyph))
        {
            var charString = character.ToString();

            using var bitmap = new SKBitmap(
                (int)bounds.Width + PADDING * 2,
                (int)bounds.Height + PADDING * 2
            );

            using var canvas = new SKCanvas(bitmap);

            canvas.DrawText(charString, PADDING + -bounds.Location.X, PADDING + -bounds.Location.Y, font, this.paint);

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

            atlas = new(size, TextureFormat.R8G8Unorm);
        }

        return atlas!;
    }

    public Dictionary<string, int>? GetCodepoints(string fontFamily, Dictionary<string, string>? externalSource)
    {
        ref var entries = ref this.codepoints.GetValueRefOrAddDefault(fontFamily, out var exists);

        if (!exists)
        {
            if (externalSource?.TryGetValue(fontFamily, out var filename) != true)
            {
                return null;
            }

            var codepoints = Path.Join(Path.GetDirectoryName(filename).AsSpan(), Path.GetFileNameWithoutExtension(filename.AsSpan())) + ".codepoints";

            if (File.Exists(codepoints))
            {
                entries = [];

                using var stream = File.Open(codepoints, FileMode.Open);
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line?.Split(' ') is [var ligature, var codepoint])
                    {
                        entries[ligature] = Convert.ToInt32(codepoint, 16);
                    }
                }
            }
        }

        return entries!;
    }

    public SKFont GetFont(string fontFamily, float fontSize, int fontWeight, Dictionary<string, string>? externalSource)
    {
        var hashcode = HashCode.Combine(fontFamily, fontSize, fontWeight);

        ref var font = ref this.fonts.GetValueRefOrAddDefault(hashcode, out var exists);

        if (!exists)
        {
            var typeface = externalSource?.TryGetValue(fontFamily, out var filename) == true
                ? SKTypeface.FromFile(filename)
                : SKTypeface.FromFamilyName(fontFamily, (SKFontStyleWeight)fontWeight, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

            font = new SKFont(typeface ?? SKTypeface.Default)
            {
                Size     = fontSize,
                Subpixel = false
            };
        }

        return font!;
    }
}
