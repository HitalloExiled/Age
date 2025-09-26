using Age.Core;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Resources;
using SkiaSharp;

namespace Age.Storage;

internal class TextStorage : Disposable
{
    private static TextStorage? singleton;

    private readonly Dictionary<int, TextureAtlas>               atlases = [];
    private readonly Dictionary<string, Dictionary<string, int>> codepoints = [];
    private readonly Dictionary<int, SKFont>                     fonts = [];
    private readonly Dictionary<int, TextureMap>                 glyphs = [];
    private readonly SKPaint                                     paint;
    private readonly VulkanRenderer                              renderer;

    public static TextStorage Singleton => singleton ?? throw new NullReferenceException();

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

    public TextureMap DrawGlyph(SKFont font, TextureAtlas atlas, scoped ReadOnlySpan<char> chars, in SKRect bounds)
    {
        const ushort PADDING = 2;

        var hashcode = string.GetHashCode(chars) ^ font.GetHashCode() ^ font.Size.GetHashCode();

        if (!this.glyphs.TryGetValue(hashcode, out var glyph))
        {
            using var bitmap   = new SKBitmap((int)bounds.Width + (PADDING * 2), (int)bounds.Height + (PADDING * 2));
            using var canvas   = new SKCanvas(bitmap);
            using var textBlob = SKTextBlob.Create(chars, font);

            canvas.DrawText(textBlob, PADDING + -bounds.Location.X, PADDING + -bounds.Location.Y, this.paint);

            var position = atlas.Pack(bitmap.GetPixelSpan().Cast<byte, uint>(), new((uint)bitmap.Width, (uint)bitmap.Height)) + PADDING;

            var atlasSize = new Point<float>(atlas.Size.Width, atlas.Size.Height);

            var uv = new UVRect
            {
                P1 = new Point<float>(position.X, position.Y) / atlasSize,
                P2 = new Point<float>(position.X + bounds.Width, position.Y) / atlasSize,
                P3 = new Point<float>(position.X + bounds.Width, position.Y + bounds.Height) / atlasSize,
                P4 = new Point<float>(position.X, position.Y + bounds.Height) / atlasSize,
            };

            this.glyphs[hashcode] = glyph = new(atlas.Texture, uv);
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
