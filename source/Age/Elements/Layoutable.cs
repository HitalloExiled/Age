using Age.Core;
using Age.Elements.Layouts;
using Age.Numerics;
using Age.Scene;

namespace Age.Elements;

public abstract partial class Layoutable : Spatial2D
{
    private CacheValue<Transform2D> transformCache;

    private Transform2D ComposedParentTransform      => (this.ComposedParentElement as Spatial2D)?.Transform ?? new();
    private Transform2D ComposedParentTransformCache => (this.ComposedParentElement as Spatial2D)?.TransformCache ?? new();
    private Transform2D Offset                       => Transform2D.CreateTranslated((this.ComposedParentElement?.Layout.ContentOffset ?? default).ToVector2<float>().InvertedX);

    internal Transform2D TransformWithOffset => this.Offset * this.Transform;

    internal abstract Layout Layout { get; }

    internal override Transform2D TransformCache
    {
        get
        {
            if (this.transformCache.Version != CacheVersion)
            {
                this.transformCache = new()
                {
                    Value   = this.Offset * this.Layout.Transform * this.ComposedParentTransformCache * this.LocalTransform,
                    Version = CacheVersion
                };
            }

            return this.transformCache.Value;
        }
    }

    public Slot? AssignedSlot { get; internal set; }

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

    public string? Slot
    {
        get;
        set
        {
            if (field != value)
            {
                if (this.Parent is Element parentElement && parentElement.ShadowTree != null)
                {
                    parentElement.UnassignSlot(field ?? "", this);
                    parentElement.AssignSlot(value ?? "", this);
                }

                field = value;
            }
        }
    }

    public Element? ComposedParentElement  => this.AssignedSlot ?? this.EffectiveParentElement;
    public Element? EffectiveParentElement => this.Parent is ShadowTree shadowTree ? shadowTree.Host : this.ParentElement;
    public Element? ParentElement          => this.Parent as Element;

    public override Transform2D Transform
    {
        get => this.Layout.Transform * (this.ComposedParentTransform * this.LocalTransform);
        set => this.LocalTransform = value * this.Transform.Inverse();
    }

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

    protected override void OnAdopted(Node parent)
    {
        base.OnAdopted(parent);

        if (parent is Element parentElement && parentElement.ShadowTree != null)
        {
            parentElement.AssignSlot(this.Slot ?? "", this);
        }
    }

    protected override void OnRemoved(Node parent)
    {
        base.OnRemoved(parent);

        if (parent is Element parentElement && parentElement.ShadowTree != null)
        {
            parentElement.UnassignSlot(this.Slot ?? "", this);
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
