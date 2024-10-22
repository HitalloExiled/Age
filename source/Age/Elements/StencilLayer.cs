using Age.Core;
using Age.Core.Extensions;
using Age.Extensions;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Storage;
using Age.Styling;
using SkiaSharp;
using System.Collections;
using ThirdParty.Vulkan.Enums;

namespace Age.Elements;

internal partial class StencilLayer(Element owner) : Disposable, IEnumerable<StencilLayer>
{
    #region 8-bytes
    private readonly SKPath path = new();

    public Element Owner { get; } = owner;

    public StencilLayer? FirstChild      { get; private set; }
    public StencilLayer? LastChild       { get; private set; }
    public StencilLayer? NextSibling     { get; private set; }
    public StencilLayer? Parent          { get; private set; }
    public StencilLayer? PreviousSibling { get; private set; }

    public MappedTexture MappedTexture { get; private set; } = new(TextureStorage.Singleton.EmptyTexture, UVRect.Normalized);
    #endregion

    #region 4-bytes
    private Transform2D transform;

    internal Transform2D Transform
    {
        get => this.transform;
        set
        {
            if (this.transform != value)
            {
                this.transform = value;

                this.MakeDirty();
            }
        }
    }

    public Size<uint> Size { get; private set; }
    #endregion

    #region 1-byte
    private bool isDirty;
    #endregion

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    private void UpdatePath(Size<uint> bounds, Border border)
    {
        this.path.Reset();

        var minRadius = uint.Min(bounds.Width, bounds.Height) / 2;

        border.Radius.LeftTop     = uint.Min(border.Radius.LeftTop,     minRadius);
        border.Radius.TopRight    = uint.Min(border.Radius.TopRight,    minRadius);
        border.Radius.RightBottom = uint.Min(border.Radius.RightBottom, minRadius);
        border.Radius.BottomLeft  = uint.Min(border.Radius.BottomLeft,  minRadius);

        bool tryCreateEllipse(uint radius, Point<uint> origin, Size<uint> thickness, float startAngle)
        {
            if (radius > thickness.Width && radius > thickness.Height)
            {
                origin.X = origin.X * (bounds.Width  - radius * 2) + radius;
                origin.Y = origin.Y * (bounds.Height - radius * 2) + radius;

                var radiusX = radius - thickness.Width;
                var radiusY = radius - thickness.Height;

                var rect = new SKRect(origin.X - radiusX, origin.Y - radiusY, origin.X + radiusX, origin.Y + radiusY);

                this.path.ArcTo(rect, startAngle, 90, false);

                return true;
            }

            return false;
        }

        this.path.MoveTo(uint.Max(border.Left.Thickness, border.Radius.LeftTop), border.Top.Thickness);

        if (border.Radius.TopRight == 0 || !tryCreateEllipse(border.Radius.TopRight, new(1, 0), new(border.Right.Thickness, border.Top.Thickness), 270))
        {
            this.path.LineTo(bounds.Width - border.Right.Thickness, border.Top.Thickness);
        }

        if (border.Radius.RightBottom == 0 || !tryCreateEllipse(border.Radius.RightBottom, new(1, 1), new(border.Right.Thickness, border.Bottom.Thickness), 0))
        {
            this.path.LineTo(bounds.Width - border.Right.Thickness, bounds.Height - border.Bottom.Thickness);
        }

        if (border.Radius.BottomLeft == 0 || !tryCreateEllipse(border.Radius.BottomLeft, new(0, 1), new(border.Left.Thickness, border.Bottom.Thickness), 90))
        {
            this.path.LineTo(border.Left.Thickness, bounds.Height - border.Bottom.Thickness);
        }

        if (border.Radius.LeftTop == 0 || !tryCreateEllipse(border.Radius.LeftTop, new(0, 0), new(border.Left.Thickness, border.Top.Thickness), 180))
        {
            this.path.LineTo(border.Left.Thickness, border.Top.Thickness);
        }
    }

    private void UpdateTexture(Size<uint> bounds, SKBitmap bitmap)
    {
        const float MIN_MARGIN_RATIO = 0.33333334f;
        const float MAX_MARGIN_RATIO = 1;

        var texture = this.MappedTexture.Texture;

        var imageSize = new Size<uint>(texture.Image.Extent.Width, texture.Image.Extent.Height);

        var isWithinMargin =
            bounds.Width / (float)imageSize.Width is >= MIN_MARGIN_RATIO and <= MAX_MARGIN_RATIO
            && bounds.Height / (float)imageSize.Height is >= MIN_MARGIN_RATIO and <= MAX_MARGIN_RATIO;

        if (texture == TextureStorage.Singleton.EmptyTexture || !isWithinMargin)
        {
            imageSize = (bounds.Cast<float>() * 1.5f).Cast<uint>();

            var textureInfo = new TextureCreateInfo
            {
                Depth     = 1,
                Format    = VkFormat.R8Unorm,
                Height    = imageSize.Height,
                ImageType = VkImageType.N2D,
                Width     = imageSize.Width,
            };

            texture = new(textureInfo);
        }

        var bufferSize = imageSize.Width * imageSize.Height;

        var pixels = bitmap.Pixels.AsSpan();

        var buffer = new byte[bufferSize];

        for (var y = 0; y < bounds.Height; y++)
        {
            for (var x = 0; x < bounds.Width; x++)
            {
                var sourceIndex = (int)(x + bounds.Width * y);
                var destinationIndex = (int)(x + imageSize.Width * y);

                buffer[destinationIndex] = pixels[sourceIndex].Alpha;
            }
        }

        texture.Update(buffer);

        var uvX = bounds.Width / (float)imageSize.Width;
        var uvY = bounds.Height / (float)imageSize.Height;

        var uv = new UVRect()
        {
            P1 = new(0, 0),
            P2 = new(uvX, 0),
            P3 = new(uvX, uvY),
            P4 = new(0, uvY),
        };

        this.MappedTexture = new(texture, uv);
    }

    protected override void Disposed(bool disposing)
    {
        if (disposing && this.MappedTexture.Texture != TextureStorage.Singleton.DefaultTexture)
        {
            VulkanRenderer.Singleton.DeferredDispose(this.MappedTexture.Texture);
        }
    }

    public void AppendChild(StencilLayer child)
    {
        if (child == this)
        {
            throw new InvalidOperationException("Cant add node to itself");
        }

        if (child.Parent != this)
        {
            child.Parent = this;

            if (this.LastChild != null)
            {
                this.LastChild.NextSibling = child;
                child.PreviousSibling = this.LastChild;

                this.LastChild = child;
            }
            else
            {
                this.FirstChild = this.LastChild = child;
            }

            child.MakeDirty();
        }
    }

    public void Detach()
    {
        if (this.Parent != null)
        {
            var parent = this.Parent;

            parent.RemoveChild(this);

            foreach (var node in this)
            {
                parent.AppendChild(node);
            }

            this.MakeDirty();
        }
    }

    public IEnumerator<StencilLayer> GetEnumerator() =>
        new Enumerator(this);

    public IEnumerable<StencilLayer> Traverse() =>
        new TraverseEnumerator(this);

    public void MakeDirty()
    {
        this.isDirty = true;

        foreach (var item in this.Traverse())
        {
            item.isDirty = true;
        }
    }

    public void RemoveChild(StencilLayer child)
    {
        if (child.Parent == this)
        {
            if (child == this.FirstChild)
            {
                this.FirstChild = child.NextSibling;
            }

            if (child == this.LastChild)
            {
                this.LastChild = child.PreviousSibling;
            }

            if (child.PreviousSibling != null)
            {
                child.PreviousSibling.NextSibling = child.NextSibling;

                if (child.NextSibling != null)
                {
                    child.NextSibling.PreviousSibling = child.PreviousSibling.NextSibling;
                }
            }
            else if (child.NextSibling != null)
            {
                child.NextSibling.PreviousSibling = null;
            }

            child.PreviousSibling = null;
            child.NextSibling     = null;
            child.Parent          = null;
        }
    }

    public override string ToString() =>
        $"{{ Owner: {this.Owner} }}";

    public void Update()
    {
        if (this.isDirty || this.transform != this.Owner.TransformCache)
        {
            var bounds = this.Owner.Layout.Boundings;
            var border = this.Owner.Layout.State.Style.Border ?? default;

            this.UpdatePath(bounds, border);

            using var bitmap = new SKBitmap((int)bounds.Width, (int)bounds.Height);
            using var canvas = new SKCanvas(bitmap);
            using var paint = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Fill,
            };

            if (this.Parent != null)
            {
                using var parentClipping = new SKPath(this.Parent.path);

                var offset = (this.Owner.TransformCache.Inverse() * this.Parent.Owner.TransformCache).Matrix;

                offset.M32 = -offset.M32;

                parentClipping.Transform(offset.ToSKMatrix());

                this.path.Op(parentClipping, SKPathOp.Intersect, this.path);
            }

            canvas.DrawPath(this.path, paint);

            this.UpdateTexture(bounds, bitmap);

            this.Size = bounds;

            this.isDirty = false;
            this.transform = this.Owner.TransformCache;
        }
    }
}
