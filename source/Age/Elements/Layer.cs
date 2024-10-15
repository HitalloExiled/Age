using Age.Core.Extensions;
using Age.Extensions;
using Age.Internal;
using Age.Numerics;
using Age.Platforms.Windows.Native.Types;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Storage;
using SkiaSharp;
using System.Collections;
using System.Drawing;
using ThirdParty.Vulkan.Enums;

namespace Age.Elements;

internal partial class Layer(Element owner) : IEnumerable<Layer>, IDisposable
{
    // 8-bytes alignment
    private readonly Element owner = owner;
    private readonly SKPath  path  = new();

    private Texture    texture = TextureStorage.Singleton.EmptyTexture;
    private LayerTree? tree;

    // 4-bytes alignment
    private Transform2D transform;

    // 2-bytes alignment
    private bool disposed;
    private bool isDirty;

    public Element Owner   => this.owner;
    public Texture Texture => this.texture;

    public Layer? FirstChild      { get; private set; }
    public Layer? LastChild       { get; private set; }
    public Layer? NextSibling     { get; private set; }
    public Layer? Parent          { get; private set; }
    public Layer? PreviousSibling { get; private set; }

    public Transform2D Transform
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

    public LayerTree? Tree
    {
        get => this.tree;
        internal set
        {
            if (value != this.tree)
            {
                static void setTree(Layer layer, LayerTree? tree)
                {
                    layer.tree = tree;

                    if (tree != null && !tree.IsDirty && layer.isDirty)
                    {
                        tree.IsDirty = true;
                    }
                }

                setTree(this, value);

                foreach (var child in this.Traverse())
                {
                    setTree(child, value);
                }
            }
        }
    }

    ~Layer() => this.Dispose(false);

    private void UpdatePath()
    {
        this.path.Reset();

        var border = this.Owner.Layout.State.Style.Border ?? default;
        var bounds = this.Owner.Layout.Boundings;

        var minRadius = uint.Min(bounds.Width, bounds.Height) / 2;

        bool tryCreateEllipse(uint radius, Point<uint> position, Size<uint> thickness, float startAngle)
        {
            var targetRadius = uint.Min(radius, minRadius);

            if (targetRadius > thickness.Width && targetRadius > thickness.Height)
            {
                var diameter = targetRadius * 2;
                var radiusX  = diameter - thickness.Width;
                var radiusY  = diameter - thickness.Height;
                var origin   = (new Point<uint>(bounds.Width, bounds.Height) - new Point<uint>(targetRadius)) * position;

                var rect = SKRect.Create(origin.X, origin.Y, radiusX, radiusY);

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
            this.path.LineTo(bounds.Width - border.Right.Thickness, bounds.Height - border.Top.Thickness);
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

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                if (this.Texture != TextureStorage.Singleton.DefaultTexture)
                {
                    VulkanRenderer.Singleton.DeferredDispose(this.Texture);
                }
            }

            this.disposed = true;
        }
    }

    public void AppendChild(Layer child)
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

            child.Tree = this.Tree;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IEnumerator<Layer> GetEnumerator() =>
        new Enumerator(this);

    public void MakeDirty()
    {
        this.isDirty = true;

        if (this.Tree != null)
        {
            this.Tree.IsDirty = true;
        }
    }

    public void Remove()
    {
        if (this.Parent != null)
        {
            var parent = this.Parent;

            parent.RemoveChild(this, true);

            foreach (var node in this)
            {
                parent.AppendChild(node);
            }
        }
        else if (this.Tree?.Root == this)
        {
            this.Tree.Root = null;
        }
    }

    public void RemoveChild(Layer child, bool keepTree = false)
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

            if (!keepTree)
            {
                child.Tree = null;
            }
        }
    }

    public override string ToString() =>
        $"{{ Owner: {this.Owner} }}";

    public IEnumerable<Layer> Traverse() =>
        new TraverseEnumerator(this);

    public void Update()
    {
        if (this.isDirty)
        {
            this.UpdatePath();

            var outter = this.Owner.Layout.Boundings;
            var border = this.Owner.Layout.Border;

            using var bitmap = new SKBitmap((int)outter.Width, (int)outter.Height);
            using var canvas = new SKCanvas(bitmap);
            using var paint  = new SKPaint
            {
                Color = SKColors.Red,
                Style = SKPaintStyle.Fill,
            };

            var ownerTransform = this.Owner.TransformCache.Matrix;

            ownerTransform.Translation = ownerTransform.Translation.InvertedY;

            var contentTransform = new Transform2D(ownerTransform) * Transform2D.CreateTranslated(border.Left, border.Top);

            this.path.Transform((this.Owner.TransformCache * Transform2D.CreateTranslated(border.Left, border.Top)).Matrix.ToSKMatrix());

            if (this.Parent != null)
            {
                var parent = this.Parent;

                while (parent != null)
                {
                    using var parentClipping = new SKPath(parent.path);
                    var parentBorder = this.Parent.Owner.Layout.Border;

                    var parentContentTransform = this.Parent.Owner.TransformCache * Transform2D.CreateTranslated(-parentBorder.Left, -parentBorder.Top);

                    parentClipping.Transform(parentContentTransform.Matrix.ToSKMatrix());

                    this.path.Op(parentClipping, SKPathOp.Intersect, this.path);

                    parent = parent.Parent;
                }
            }

            //this.path.Transform((this.Owner.TransformCache * Transform2D.CreateTranslated(border.Left, border.Top)).Matrix.ToSKMatrix());

            canvas.DrawPath(this.path, paint);

            var buffer = new byte[bitmap.Pixels.Length];

            var pixels = bitmap.Pixels.AsSpan().Cast<SKColor, byte>();

            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = pixels[i * 4 + 2];
            }

            Common.SaveImage(bitmap, $"{this.Owner.Name}.png");

            if (this.texture == TextureStorage.Singleton.EmptyTexture || this.texture.Image.Extent.Width != outter.Width || this.texture.Image.Extent.Height != outter.Height)
            {
                var textureInfo = new TextureCreateInfo
                {
                    Depth     = 1,
                    Format    = VkFormat.R8Unorm,
                    Width     = outter.Width,
                    Height    = outter.Height,
                    ImageType = VkImageType.N2D,
                };

                this.texture = VulkanRenderer.Singleton.CreateTexture(textureInfo);
            }

            this.Texture.Update(buffer);

            this.isDirty = false;
        }
    }
}
