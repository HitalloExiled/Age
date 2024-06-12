using Age.Numerics;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Storage;

public class TextureStorage : ITextureStorage
{
    private readonly Dictionary<Texture, UniformSet> textureSets = [];
    private readonly VulkanRenderer renderer;

    public Texture DefaultTexture { get; }
    public Sampler DefaultSampler { get; }
    private bool disposed;

    public TextureStorage(VulkanRenderer renderer)
    {
        this.renderer = renderer;

        const int DEFAULT_SIZE = 8;

        var textureCreateInfo = new TextureCreateInfo
        {
            Format    = VkFormat.B8G8R8A8Unorm,
            ImageType = VkImageType.N2D,
            Width     = DEFAULT_SIZE,
            Height    = DEFAULT_SIZE,
            Depth     = 1,
        };

        this.DefaultTexture = this.renderer.CreateTexture(textureCreateInfo);
        this.DefaultSampler = renderer.CreateSampler();

        var bytesPerColor = (int)ColorMode.RGBA;

        Span<byte> colorBuffer = Color.Margenta.ToByteArray();
        Span<byte> imageBuffer = stackalloc byte[DEFAULT_SIZE * DEFAULT_SIZE * bytesPerColor];

        for (var i = 0; i < imageBuffer.Length; i += bytesPerColor)
        {
            colorBuffer.CopyTo(imageBuffer[i..(i + bytesPerColor)]);
        }

        this.DefaultTexture.Update(imageBuffer);
    }

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.renderer.DeferredDispose(this.DefaultTexture);
            this.renderer.DeferredDispose(this.DefaultSampler);
            this.renderer.DeferredDispose(this.textureSets.Values);

            this.disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
