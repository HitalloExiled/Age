using System.Reflection;
using Age.Core;
using Age.Core.Collections;
using Age.Numerics;
using Age.Rendering.Vulkan;
using Age.Resources;
using SkiaSharp;
using ThirdParty.Skia.Svg;

namespace Age.Storage;

internal class IconStorage : Disposable
{
    private static IconStorage? singleton;

    public static IconStorage Singleton => field ?? throw new NullReferenceException();

    private readonly TextureAtlas                       atlas = new(new(512), TextureFormat.R8G8Unorm);
    private readonly KeyedList<Icon, MappedTexture> icons = [];
    private readonly VulkanRenderer                     renderer;

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
        var assembly = Assembly.GetExecutingAssembly();

        foreach (var resource in assembly.GetManifestResourceNames())
        {
            using var stream = assembly.GetManifestResourceStream(resource)!;

            var svg = new SKSvg();

            svg.Load(stream);

            var size = new SKSize(24, 24);

            using var bitmap = new SKBitmap((int)size.Width, (int)size.Height);
            using var canvas = new SKCanvas(bitmap);

            canvas.Clear(SKColors.Transparent);

            canvas.DrawPicture(svg.Picture);

            var position = this.atlas.Pack(bitmap.GetPixelSpan(), new(24));

            var span       = resource.AsSpan();
            var enumerator = resource.AsSpan().Split('.');

            enumerator.MoveNext();
            enumerator.MoveNext();
            enumerator.MoveNext();
            enumerator.MoveNext();

            var atlasSize = new Point<float>(this.atlas.Size.Width, this.atlas.Size.Height);

            var uv = new UVRect
            {
                P1 = new Point<float>(position.X, position.Y) / atlasSize,
                P2 = new Point<float>(position.X + size.Width, position.Y) / atlasSize,
                P3 = new Point<float>(position.X + size.Width, position.Y + size.Height) / atlasSize,
                P4 = new Point<float>(position.X, position.Y + size.Height) / atlasSize,
            };

            var name = Icon.Parse(span[enumerator.Current]);

            this.icons[name] = new(this.atlas.Texture, uv);
        }
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.renderer.DeferredDispose(this.atlas);
        }
    }

    public MappedTexture GetIcon(Icon name) =>
        this.icons[name];
}
