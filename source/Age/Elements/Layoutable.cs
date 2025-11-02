using Age.Numerics;
using Age.Platforms.Display;
using Age.Scenes;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Elements;

public abstract class Layoutable : Spatial2D
{
    internal const string DEFAULT_FONT_FAMILY = "Segoi UI";
    internal const ushort DEFAULT_FONT_SIZE   = 16;

    private Transform2D CachedComposedParentTransform => (this.CompositeParentElement as Layoutable)?.CachedTransform ?? Transform2D.Identity;
    private Transform2D CombinedTransform             => this.LayoutTransform * this.LocalTransform * this.ParentContentOffset;
    private Transform2D CompositeParentTransform      => (this.CompositeParentElement as Layoutable)?.Transform ?? Transform2D.Identity;
    private Transform2D ParentContentOffset           => Transform2D.CreateTranslated((this.CompositeParentElement?.ContentOffset ?? default).ToVector2<float>().InvertedX);

    private protected virtual StencilLayer? ContentStencilLayer { get; }
    private protected virtual Transform2D   LayoutTransform => Transform2D.CreateTranslated(this.Offset);

    protected static Element?     ActiveScrollBarTarget  { get; set; }
    protected static Text?        ActiveText             { get; set; }
    protected static bool         IsDraggingScrollBarX   { get; set; }
    protected static bool         IsDraggingScrollBarY   { get; set; }
    protected static bool         IsHoveringScrollBarX   { get; set; }
    protected static bool         IsHoveringScrollBarY   { get; set; }
    protected static bool         IsHoveringText         { get; set; }
    protected static Point<float> ScrollBarClickPosition { get; set; }

    protected static bool IsHoveringScrollBar => IsHoveringScrollBarX || IsHoveringScrollBarY;

    internal static bool IsDraggingScrollBar => IsDraggingScrollBarX || IsDraggingScrollBarY;
    internal static bool IsSelectingText     => ActiveText != null;

    internal bool IsDirty { get; private set; }

    internal int        BaseLine  { get; private protected set; } = -1;
    internal Size<uint> Boundings { get; private protected set; }

    internal uint           LineHeight { get; set; }
    internal Vector2<float> Offset     { get; set; }

    internal virtual StencilLayer? StencilLayer { get; set; }

    internal sealed override Transform2D CachedTransform
    {
        get
        {
            if (this.TransformCache.IsInvalid)
            {
                this.TransformCache = new(this.CombinedTransform * this.CachedComposedParentTransform);
            }

            return this.TransformCache.Value;
        }
    }

    internal abstract bool IsParentDependent { get; }

    public sealed override Transform2D Transform
    {
        get => this.CombinedTransform * this.CompositeParentTransform;
        set => this.LocalTransform = this.LayoutTransform.Inverse() * value * this.CompositeParentTransform.Inverse() * this.ParentContentOffset.Inverse();
    }

    public Element? CompositeParentElement => this.CompositeParent as Element;

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

    private protected static ulong CombineIds(int elementIndex, int childIndex) =>
        ((ulong)childIndex << 24) | ((uint)elementIndex);

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


    private protected override void OnConnectedInternal()
    {
        base.OnConnectedInternal();

        this.StencilLayer = this.CompositeParentElement?.ContentStencilLayer;
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnectingInternal();

        this.StencilLayer = null;
    }

    private protected void RequestUpdate(bool affectsBoundings)
    {
        var tree = this.Scene?.Viewport?.Window?.Tree;

        for (var current = this; ; current = current.CompositeParentElement!)
        {
            if (current.IsDirty || !current.Visible)
            {
                return;
            }

            current.MakeDirty();

            var stopPropagation = (!current.IsParentDependent && !affectsBoundings) || current.CompositeParentElement == null;

            if (stopPropagation)
            {
                tree?.AddDeferredUpdate(current.UpdateDirtyLayout);

                return;
            }
        }
    }

    private protected void SetCursor(Cursor? cursor)
    {
        Debug.Assert(this.Scene?.Viewport?.Window != null);

        this.Scene.Viewport.Window.Cursor = cursor ?? default;
    }

    private protected void UpdateLayoutIndependentAncestor()
    {
        if (this.IsDirty)
        {
            this.GetIndependentAncestor().UpdateLayout();
        }
    }

    internal static Layoutable? GetCommonComposedAncestor(Layoutable left, Layoutable right)
    {
        var leftCompositeParentElement  = left.CompositeParentElement;
        var rightCompositeParentElement = right.CompositeParentElement;

        if (leftCompositeParentElement == rightCompositeParentElement)
        {
            return leftCompositeParentElement;
        }
        else if (left == rightCompositeParentElement)
        {
            return left;
        }
        else if (leftCompositeParentElement == right)
        {
            return right;
        }
        else
        {
            var leftDepth  = 0;
            var rightDepth = 0;

            Layoutable? currentLeft  = leftCompositeParentElement;
            Layoutable? currentRight = rightCompositeParentElement;

            while (currentLeft != null)
            {
                leftDepth++;
                currentLeft  = currentLeft.CompositeParentElement;
            }

            while (currentRight != null)
            {
                rightDepth++;
                currentRight  = currentRight.CompositeParentElement;
            }

            currentLeft  = left;
            currentRight = right;

            while (leftDepth > rightDepth)
            {
                currentLeft = currentLeft.CompositeParentElement!;
                leftDepth--;
            }

            while (leftDepth < rightDepth)
            {
                currentRight = currentRight.CompositeParentElement!;
                rightDepth--;
            }

            while (currentLeft != currentRight)
            {
                currentLeft  = currentLeft.CompositeParentElement;
                currentRight = currentRight.CompositeParentElement;

                if (currentLeft == null || currentRight == null)
                {
                    return null;
                }
            }

            return currentLeft;
        }
    }

    internal static ComposedPath GetComposedPathBetween(Layoutable left, Layoutable right)
    {
        var leftToAncestor  = new List<Layoutable>();
        var rightToAncestor = new List<Layoutable>();

        GetComposedPathBetween(leftToAncestor, rightToAncestor, left, right);

        return new(leftToAncestor, rightToAncestor);
    }

    internal static void GetComposedPathBetween(List<Layoutable> leftToAncestor, List<Layoutable> rightToAncestor, Layoutable left, Layoutable right)
    {
        const string ERROR_MESSAGE = "The specified elements do not share a common ancestor in the composed tree.";

        leftToAncestor.Clear();
        rightToAncestor.Clear();

        leftToAncestor.Add(left);
        rightToAncestor.Add(right);

        var leftCompositeParentElement  = left.CompositeParentElement;
        var rightCompositeParentElement = right.CompositeParentElement;

        if (leftCompositeParentElement == rightCompositeParentElement)
        {
            if (leftCompositeParentElement == null)
            {
                throw new InvalidOperationException(ERROR_MESSAGE);
            }

            leftToAncestor.Add(leftCompositeParentElement);
            rightToAncestor.Add(leftCompositeParentElement);
        }
        else if (left == rightCompositeParentElement)
        {
            rightToAncestor.Add(left);
        }
        else if (leftCompositeParentElement == right)
        {
            leftToAncestor.Add(right);
        }
        else
        {
            var leftDepth  = 0;
            var rightDepth = 0;

            Layoutable? currentLeft  = leftCompositeParentElement;
            Layoutable? currentRight = rightCompositeParentElement;

            while (currentLeft != null)
            {
                leftDepth++;
                currentLeft = currentLeft.CompositeParentElement;
            }

            while (currentRight != null)
            {
                rightDepth++;
                currentRight  = currentRight.CompositeParentElement;
            }

            currentLeft  = left;
            currentRight = right;

            while (leftDepth > rightDepth)
            {
                currentLeft = currentLeft.CompositeParentElement!;
                leftDepth--;

                leftToAncestor.Add(currentLeft);
            }

            while (leftDepth < rightDepth)
            {
                currentRight = currentRight.CompositeParentElement!;
                rightDepth--;

                rightToAncestor.Add(currentRight);
            }

            while (currentLeft != currentRight)
            {
                currentLeft  = currentLeft.CompositeParentElement;
                currentRight = currentRight.CompositeParentElement;

                if (currentLeft == null || currentRight == null)
                {
                    leftToAncestor.Clear();
                    rightToAncestor.Clear();

                    throw new InvalidOperationException(ERROR_MESSAGE);
                }

                leftToAncestor.Add(currentLeft);
                rightToAncestor.Add(currentRight);
            }
        }
    }

    internal int GetEffectiveDepth()
    {
        var depth = 0;

        var node = this.CompositeParentElement;

        while (node != null)
        {
            depth++;
            node = node.CompositeParentElement;
        }

        return depth;
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

        var transform = this.Transform;

        var size     = this.Boundings.Cast<int>();
        var position = new Point<int>((int)transform.Position.X, -(int)transform.Position.Y);

        return new(size, position);
    }
}
