using System.Runtime.CompilerServices;
using Age.Core;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Scene;

namespace Age.Elements;

public abstract class Layoutable : Spatial2D
{
    private CacheValue<Transform2D> transformCache;

    internal const string DEFAULT_FONT_FAMILY = "Segoi UI";
    internal const ushort DEFAULT_FONT_SIZE = 16;

    private Transform2D ComposedParentTransform      => (this.ComposedParentElement as Spatial2D)?.Transform ?? Transform2D.Identity;
    private Transform2D ComposedParentTransformCache => (this.ComposedParentElement as Spatial2D)?.TransformCache ?? Transform2D.Identity;
    private Transform2D ParentContentOffset          => Transform2D.CreateTranslated((this.ComposedParentElement?.ContentOffset ?? default).ToVector2<float>().InvertedX);

    private protected virtual StencilLayer? ContentStencilLayer { get; }
    private protected virtual Transform2D   LayoutTransform => Transform2D.CreateTranslated(this.Offset);

    internal static bool IsHoveringText   { get; set; }
    internal static bool IsHoveringScroll { get; set; }
    internal static bool IsScrollingX     { get; set; }
    internal static bool IsScrollingY     { get; set; }
    internal static bool IsSelectingText  { get; set; }

    internal static bool IsScrolling => IsScrollingX || IsScrollingY;

    internal bool IsDirty { get; private set; }

    internal int        BaseLine  { get; private protected set; } = -1;
    internal Size<uint> Boundings { get; private protected set; }

    internal uint           LineHeight { get; set; }
    internal Vector2<float> Offset     { get; set; }

    internal virtual bool          Hidden       { get; set; }
    internal virtual StencilLayer? StencilLayer { get; set; }

    internal abstract bool IsParentDependent { get; }

    public Slot? AssignedSlot { get; internal set; }

    public string? Slot
    {
        get;
        set
        {
            if (field != value)
            {
                if (this.Parent is Element parentElement && parentElement.ShadowTree != null)
                {
                    parentElement.ShadowTree.UnassignSlot(field ?? "", this);
                    parentElement.ShadowTree.AssignSlot(value   ?? "", this);
                }

                field = value;
            }
        }
    }

    public override Transform2D Transform
    {
        get => this.LayoutTransform * (this.LocalTransform * this.ComposedParentTransform);
        set => this.LocalTransform = value * this.Transform.Inverse();
    }

    public Element? ComposedParentElement  => this.AssignedSlot ?? this.EffectiveParentElement;
    public Element? EffectiveParentElement => this.Parent is ShadowTree shadowTree ? shadowTree.Host: this.Parent as Element;

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

    public Element? ParentElement => this.Parent as Element;

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

    internal override Transform2D TransformCache
    {
        get
        {
            if (this.transformCache.Version != CacheVersion)
            {
                this.transformCache = new()
                {
                    Value = this.ParentContentOffset * this.LayoutTransform * this.LocalTransform * this.ComposedParentTransformCache,
                    Version = CacheVersion
                };
            }

            return this.transformCache.Value;
        }
    }

    internal Transform2D TransformWithOffset => this.ParentContentOffset * this.Transform;

    private protected static ulong CombineIds(int elementIndex, int childIndex) =>
        ((ulong)childIndex << 24) | ((uint)elementIndex);

    private protected static Element? GetStyleSource(Node? node) =>
        node is not ShadowTree shadowTree
            ? (node as Element)
            : shadowTree.InheritsHostStyle
                ? shadowTree.Host
                : null;

    private protected Layoutable GetIndependentAncestor()
    {
        var current = this;

        while (current.IsParentDependent)
        {
            if (current.Parent is not Layoutable parent)
            {
                break;
            }

            current = parent;
        }

        return current;
    }

    private protected void MakeDirty() =>
        this.IsDirty = true;

    private protected void MakePristine() =>
        this.IsDirty = false;

    protected override void OnAdopted(Node parent)
    {
        base.OnAdopted(parent);

        if (parent is Element parentElement && parentElement.ShadowTree != null)
        {
            parentElement.ShadowTree.AssignSlot(this.Slot ?? "", this);
        }
    }

    protected override void OnConnected(RenderTree renderTree) =>
        this.StencilLayer = this.ComposedParentElement?.ContentStencilLayer;

    protected override void OnDisconnected(RenderTree renderTree) =>
        this.StencilLayer = null;

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.OnDisposed();
        }
    }

    protected override void OnRemoved(Node parent)
    {
        base.OnRemoved(parent);

        if (parent is Element parentElement && parentElement.ShadowTree != null)
        {
            parentElement.ShadowTree.UnassignSlot(this.Slot ?? "", this);
        }
    }

    private protected void RequestUpdate(bool affectsBoundings)
    {
        for (var current = this; ; current = current.ComposedParentElement!)
        {
            if (current.IsDirty || current.Hidden)
            {
                return;
            }

            current.MakeDirty();

            var stopPropagation = (!current.IsParentDependent && !affectsBoundings) || current.ComposedParentElement == null;

            if (stopPropagation)
            {
                if (current.Tree is RenderTree renderTree)
                {
                    renderTree.AddDeferredUpdate(current.UpdateDirtyLayout);
                }

                return;
            }
        }
    }

    private protected void SetCursor(Cursor? cursor)
    {
        if (this.Tree is RenderTree renderTree)
        {
            renderTree.Window.Cursor = cursor ?? default;
        }
    }

    private protected void UpdateLayoutIndependentAncestor()
    {
        if (this.IsDirty)
        {
            this.GetIndependentAncestor().UpdateLayout();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void UpdateDirtyLayout()
    {
        if (this.IsDirty)
        {
            this.UpdateLayout();
            this.MakePristine();
        }
    }

    internal abstract void UpdateLayout();

    public Rect<int> GetUpdatedBoundings()
    {
        this.UpdateLayoutIndependentAncestor();

        var transform = this.TransformWithOffset;

        var size     = this.Boundings.Cast<int>();
        var position = new Point<int>((int)transform.Position.X, -(int)transform.Position.Y);

        return new(size, position);
    }
}
