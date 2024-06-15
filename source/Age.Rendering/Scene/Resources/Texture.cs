using Age.Rendering.Vulkan;

using TextureResource = Age.Rendering.Resources.Texture;
using SamplerResource = Age.Rendering.Resources.Sampler;
using Age.Numerics;

namespace Age.Rendering.Scene.Resources;

public class Texture(Image image, bool imageOwner) : Disposable
{
    public static Texture Default { get; } = CreateDefaultTexture();

    private readonly bool imageOwner = imageOwner;

    private readonly TextureResource resource = VulkanRenderer.Singleton.CreateTexture(image, imageOwner);

    public Image Image { get; } = image;

    internal SamplerResource Sampler { get; set; } = Container.Singleton.TextureStorage.DefaultSampler;

    private static Texture CreateDefaultTexture()
    {
        const short DEFAULT_SIZE    = 8;
        const short BYTES_PER_COLOR = 4;

        Span<byte> colorBuffer = Color.Margenta.ToByteArray();
        Span<byte> imageBuffer = stackalloc byte[DEFAULT_SIZE * DEFAULT_SIZE * BYTES_PER_COLOR];

        for (var i = 0; i < imageBuffer.Length; i += BYTES_PER_COLOR)
        {
            colorBuffer.CopyTo(imageBuffer[i..(i + BYTES_PER_COLOR)]);
        }

        var image = new Image(imageBuffer, new((uint)DEFAULT_SIZE));

        return new Texture(image, true);
    }

    public static Texture FromImage(Image image) =>
        new(image, false);

    public static Texture Load(string path)
    {
        var image = Image.Load(path);

        return new(image, true);
    }

    protected override void OnDispose()
    {
        if (this.imageOwner)
        {
            this.Image.Dispose();
        }
    }

    public static implicit operator TextureResource(Texture value) =>
        value.resource;
}
