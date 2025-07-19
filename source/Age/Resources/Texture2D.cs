using Age.Numerics;
using Age.Rendering.Extensions;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Storage;
using SkiaSharp;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Resources;

public sealed class Texture2D : Resource
{
    private readonly bool imageOwner;

    internal Image       Image     { get; }
    internal VkImageView ImageView { get; }
    internal Sampler     Sampler   { get; } = new();

    public static Texture2D Default { get; } = CreateAndStore(nameof(Default), Color.Margenta);
    public static Texture2D Empty   { get; } = CreateAndStore(nameof(Empty), default);

    public TextureFormat Format { get; }

    public Size<uint> Size => this.Image.Extent.ToSize();

    internal Texture2D(Image image, TextureFormat format)
    {
        this.Image     = image;
        this.ImageView = CreateImageView(image);
        this.Format    = format;
    }

    public Texture2D(in Size<uint> size, TextureFormat format = TextureFormat.B8G8R8A8Unorm, TextureSamples samples = TextureSamples.N1, uint mipmap = 1)
    {
        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = size.ToExtent3D(),
            Format        = (VkFormat)format,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = mipmap,
            Samples       = (VkSampleCountFlags)samples,
            Tiling        = VkImageTiling.Optimal,
            Usage         = VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled,
        };

        this.imageOwner = true;
        this.Image      = new(imageCreateInfo);
        this.ImageView  = CreateImageView(this.Image);
        this.Format     = format;
    }

    public Texture2D(in Size<uint> size, in Color clearColor, TextureFormat format = TextureFormat.B8G8R8A8Unorm, TextureSamples samples = TextureSamples.N1, uint mipmap = 1)
    : this(size, format, samples, mipmap) => this.Image.ClearColor(clearColor, VkImageLayout.ShaderReadOnlyOptimal);

    public Texture2D(in Size<uint> size, scoped ReadOnlySpan<byte> buffer, TextureFormat format = TextureFormat.B8G8R8A8Unorm, TextureSamples samples = TextureSamples.N1, uint mipmap = 1)
    : this(size, format, samples, mipmap) => this.Image.Update(buffer);

    private static Texture2D CreateAndStore(string name, in Color color)
    {
        var texture = new Texture2D(new(1), color);

        TextureStorage.Singleton.Add(name, texture);

        return texture;
    }

    private static VkImageView CreateImageView(Image image)
    {
        var imageViewCreateInfo = new VkImageViewCreateInfo
        {
            Format = image.Format,
            Image  = image.Instance.Handle,
            SubresourceRange = new()
            {
                AspectMask = VkImageAspectFlags.Color,
                LayerCount = 1,
                LevelCount = 1,
            },
            ViewType = VkImageViewType.N2D,
        };

        return VulkanRenderer.Singleton.Context.Device.CreateImageView(imageViewCreateInfo);
    }
    protected override void OnDisposed()
    {
        if (this.imageOwner)
        {
            VulkanRenderer.Singleton.DeferredDispose(this.Image);
        }

        VulkanRenderer.Singleton.DeferredDispose(this.ImageView);
        VulkanRenderer.Singleton.DeferredDispose(this.Sampler);
    }

    public static Texture2D Load(string path)
    {
        using var stream = File.OpenRead(path);

        var bitmap = SKBitmap.Decode(stream);

        var buffer = bitmap.GetPixelSpan();

        return new(new((uint)bitmap.Width, (uint)bitmap.Height), buffer);
    }

    public void Update(scoped ReadOnlySpan<byte> buffer) =>
        this.Image.Update(buffer);
}
