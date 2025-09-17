using Age.Numerics;
using Age.Rendering.Extensions;
using Age.Rendering.Resources;
using Age.Storage;
using SkiaSharp;

namespace Age.Resources;

public partial class Texture2D : Texture
{
    public static Texture2D Default { get; } = CreateAndStore(nameof(Default), Color.Margenta);
    public static Texture2D Empty   { get; } = CreateAndStore(nameof(Empty), default);

    public Size<uint> Size => this.Extent.ToSize();

    public Texture2D(in CreateInfo createInfo) : base(createInfo)
    { }

    public Texture2D(in CreateInfo createInfo, in Color clearColor) : base(createInfo, clearColor)
    { }

    public Texture2D(in CreateInfo createInfo, ReadOnlySpan<byte> buffer) : base(createInfo, buffer)
    { }

    internal Texture2D(Image image, bool owner, TextureAspect aspect) : base(image, owner, aspect)
    { }

    private static Texture2D CreateAndStore(string name, in Color color)
    {
        var texture = new Texture2D(new CreateInfo { Size = new(1) }, color);

        TextureStorage.Singleton.Add(name, texture);

        return texture;
    }

    public static Texture2D Load(string path)
    {
        using var stream = File.OpenRead(path);

        var bitmap = SKBitmap.Decode(stream);

        var buffer = bitmap.GetPixelSpan();

        return new(new CreateInfo { Size = new((uint)bitmap.Width, (uint)bitmap.Height) }, buffer);
    }
}
