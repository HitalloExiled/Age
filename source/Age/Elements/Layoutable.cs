using Age.Core;
using Age.Elements.Layouts;
using Age.Numerics;
using Age.Scene;

namespace Age.Elements;

public abstract partial class Layoutable : Spatial2D
{
    #region 8-bytes
    internal abstract Layout Layout { get; }

    public Slot? AssignedSlot { get; internal set; }
    #endregion

    #region 4-bytes
    private CacheValue<Transform2D> transformCache;
    #endregion

    public Element? FirstElementChild
    {
        get
        {
            for (var node = this.FirstChild; node != null; node = node?.NextSibling)
            {
                if (node is Element element)
                {
                    return element;
                }
            }

            return null;
        }
    }

    public Element? LastElementChild
    {
        get
        {
            for (var node = this.LastChild; node != null; node = node?.PreviousSibling)
            {
                if (node is Element element)
                {
                    return element;
                }
            }

            return null;
        }
    }

    public Element? NextElementSibling
    {
        get
        {
            for (var node = this.NextSibling; node != null; node = node?.NextSibling)
            {
                if (node is Element element)
                {
                    return element;
                }
            }

            return null;
        }
    }

    public Element? PreviousElementSibling
    {
        get
        {
            for (var node = this.PreviousSibling; node != null; node = node?.PreviousSibling)
            {
                if (node is Element element)
                {
                    return element;
                }
            }

            return null;
        }
    }

    private Transform2D Offset => Transform2D.CreateTranslated((this.ParentElementOrShadowTreeHost?.Layout.ContentOffset ?? default).ToVector2<float>().InvertedX);

    internal Transform2D TransformWithOffset => this.Offset * this.Transform;

    internal override Transform2D TransformCache
    {
        get
        {
            if (this.transformCache.Version != CacheVersion)
            {
                this.transformCache = new()
                {
                    Value   = this.Offset * this.Layout.Transform * base.TransformCache,
                    Version = CacheVersion
                };
            }

            return this.transformCache.Value;
        }
    }

    internal Element? ParentElementOrShadowTreeHost => this.Parent is ShadowTree shadowTree ? shadowTree.Host : this.ParentElement;

    public override Transform2D Transform
    {
        get => this.Layout.Transform * base.Transform;
        set => this.LocalTransform = value * this.Transform.Inverse();
    }

    public Element? ParentElement => this.Parent as Element;

    private protected Layout GetIndependentLayoutAncestor()
    {
        var current = this;

        while (current.Layout.IsParentDependent)
        {
            if (current.Parent is not Layoutable parent)
            {
                break;
            }

            current = parent;
        }

        return current.Layout;
    }

    private protected void UpdateIndependentAncestorLayout()
    {
        if (this.Layout.IsDirty)
        {
            this.GetIndependentLayoutAncestor().Update();
        }
    }

    public Rect<int> GetBoundings()
    {
        this.UpdateIndependentAncestorLayout();

        var transform = this.TransformWithOffset;

        var size     = this.Layout.Boundings.Cast<int>();
        var position = new Point<int>((int)transform.Position.X, -(int)transform.Position.Y);

        return new(size, position);
    }

    public ComposedTreeTraversalEnumerator GetComposedTreeTraversalEnumerator() =>
        new(this);
}
