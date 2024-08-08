using Age.Core;
using Age.Numerics;
using Age.Rendering.Vulkan;
using Age.Storage;

using TextureResource = Age.Rendering.Resources.Texture;
using SamplerResource = Age.Rendering.Resources.Sampler;

namespace Age.Resources;

public class Texture : Disposable
{
    public static Texture Default { get; } = CreateDefaultTexture();

    private readonly bool imageOwner;

    private readonly TextureResource resource;

    public Texture(Image image, bool imageOwner)
    {
        this.imageOwner = imageOwner;
        this.resource   = VulkanRenderer.Singleton.CreateTexture(image, imageOwner);
        this.Image      = image;

        TextureStorage.Singleton.Add(Guid.NewGuid().ToString(), this.resource);
    }

    public Image Image { get; }

    internal SamplerResource Sampler { get; set; } = TextureStorage.Singleton.DefaultSampler;

    private static Texture CreateDefaultTexture()
    {
        const short DEFAULT_SIZE    = 8;
        const short BYTES_PER_COLOR = 4;

        var margenta = Color.Margenta;

        Span<byte> colorBuffer = Color.Margenta.ToByteArray();
        Span<byte> imageBuffer = stackalloc byte[DEFAULT_SIZE * DEFAULT_SIZE * BYTES_PER_COLOR];

        for (var i = 0; i < imageBuffer.Length; i += BYTES_PER_COLOR)
        {
            colorBuffer.CopyTo(imageBuffer[i..(i + BYTES_PER_COLOR)]);
        }

        var image = new Image(imageBuffer, new((uint)DEFAULT_SIZE));

        var texture = new Texture(image, true);

        TextureStorage.Singleton.Add("Default", texture);

        return texture;
    }

    public static Texture FromImage(Image image) =>
        new(image, false);

    public static Texture Load(string path)
    {
        var image = Image.Load(path);

        return new(image, true);
    }

    protected override void Disposed()
    {
        if (this.imageOwner)
        {
            this.Image.Dispose();
        }
    }

    public static implicit operator TextureResource(Texture value) =>
        value.resource;
}
