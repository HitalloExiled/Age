using Age.Core;
using Age.Elements.Layouts;
using Age.Numerics;
using Age.Scene;

namespace Age.Elements;

public abstract class Layoutable : Spatial2D
{
    internal abstract Layout Layout { get; }

    private CacheValue<Transform2D> transformCache;

    private Transform2D Offset => Transform2D.CreateTranslated((this.ParentElement?.Layout.ContentOffset ?? default).ToVector2<float>().InvertedX);

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
}
