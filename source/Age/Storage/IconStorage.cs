using System.Reflection;
using Age.Core;
using Age.Rendering.Vulkan;
using Age.Resources;
using SkiaSharp.Extended.Svg;

namespace Age.Storage;

internal partial class IconStorage : Disposable
{
    private static IconStorage? singleton;

    public static IconStorage Singleton => field ?? throw new NullReferenceException();

    private readonly TextureAtlas               atlas = new(new(512), TextureFormat.R8G8Unorm);
    private readonly Dictionary<IconName, Icon> icons = [];
    private readonly VulkanRenderer             renderer;

    public IconStorage(VulkanRenderer renderer)
    {
        if (singleton != null)
        {
            throw new InvalidOperationException($"Only one single instace of {nameof(TextStorage)} is allowed");
        }

        singleton = this;

        this.renderer = renderer;

        this.BuildAtlas();
    }

    private void BuildAtlas()
    {
        var assembly  = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames();

        foreach (var resource in resources)
        {
            using var stream = assembly.GetManifestResourceStream(resource);

            var svg = new SKSvg();

            // svg.Load(stream);

            // var size = new SKSize(24, 24);

            // using var bitmap = new SKBitmap((int)size.Width, (int)size.Height);
            // using var canvas = new SKCanvas(bitmap);

            // canvas.Clear(SKColors.Transparent);

            // canvas.DrawPicture(svg.Picture);

            // this.atlas.Pack(bitmap.GetPixelSpan(), new(24));
        }
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.renderer.DeferredDispose(this.atlas);
        }
    }

    // public Glyph DrawGlyph(SKFont font, TextureAtlas atlas, char character, in SKRect bounds)
    // {
    //     const ushort PADDING = 2;

    //     var hashcode = character.GetHashCode() ^ font.GetHashCode() ^ font.Size.GetHashCode();

    //     if (!this.glyphs.TryGetValue(hashcode, out var glyph))
    //     {
    //         var charString = character.ToString();

    //         using var bitmap = new SKBitmap(
    //             (int)bounds.Width + PADDING * 2,
    //             (int)bounds.Height + PADDING * 2
    //         );

    //         using var canvas = new SKCanvas(bitmap);

    //         canvas.DrawText(charString, PADDING + -bounds.Location.X, PADDING + -bounds.Location.Y, font, this.paint);

    //         var position = atlas.Pack(bitmap.GetPixelSpan().Cast<byte, uint>(), new((uint)bitmap.Width, (uint)bitmap.Height));

    //         this.glyphs[hashcode] = glyph = new()
    //         {
    //             Atlas     = atlas,
    //             Character = character,
    //             Position  = position + PADDING,
    //             Size      = new((uint)bounds.Width, (uint)bounds.Height),
    //         };
    //     }

    //     return glyph;
    // }
}
