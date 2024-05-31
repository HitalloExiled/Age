using Age.Numerics;
using Age.Rendering.Enums;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Rendering.Vulkan.Uniforms;

namespace Age.Rendering.Storage;

internal class TextureStorage : ITextureStorage
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

        this.DefaultTexture = this.CreateTexture(new(DEFAULT_SIZE, DEFAULT_SIZE), ColorMode.RGBA, TextureType.N2D);
        this.DefaultSampler = renderer.CreateSampler();

        var bytesPerColor = (int)ColorMode.RGBA;

        Span<byte> colorBuffer = Color.Margenta.ToByteArray();
        Span<byte> imageBuffer = stackalloc byte[DEFAULT_SIZE * DEFAULT_SIZE * bytesPerColor];

        for (var i = 0; i < imageBuffer.Length; i += bytesPerColor)
        {
            colorBuffer.CopyTo(imageBuffer[i..(i + bytesPerColor)]);
        }

        this.renderer.UpdateTexture(this.DefaultTexture, imageBuffer);
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

    public Texture CreateTexture(Size<uint> size, ColorMode colorMode, TextureType textureType)
    {
        var textureCreate = new TextureCreate
        {
            Depth = 1,
            Width = size.Width,
            Height = size.Height,
            TextureType = textureType,
            ColorMode = colorMode,
        };

        return this.renderer.CreateTexture(textureCreate);
    }

    public void FreeTexture(Texture texture)
    {
        this.renderer.DeferredDispose(texture);

        if (this.textureSets.Remove(texture, out var uniformSet))
        {
            uniformSet.Dispose();
        }
    }

    public UniformSet GetUniformSet(Shader shader, Texture texture, Sampler sampler)
    {
        if (!this.textureSets.TryGetValue(texture, out var uniformSet))
        {
            var uniform = new CombinedImageSamplerUniform
            {
                Binding = 0,
                Images =
                [
                    new()
                    {
                        Sampler = sampler,
                        Texture = texture,
                    }
                ]
            };

            this.textureSets[texture] = uniformSet = this.renderer.CreateUniformSet(shader, [uniform]);
        }

        return uniformSet;
    }
}
