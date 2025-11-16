using Age.Commands;
using Age.Core;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Scenes;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Elements;

public abstract class Layoutable : Spatial<Command2D, Matrix3x2<float>>
{
    internal const string DEFAULT_FONT_FAMILY = "Segoi UI";
    internal const ushort DEFAULT_FONT_SIZE   = 16;

    private CacheValue<Matrix3x2<float>> matrixCache;

    private Matrix3x2<float> CachedCompositeParentMatrix => (this.CompositeParentElement as Layoutable)?.CachedMatrix ?? Matrix3x2<float>.Identity;
    private Matrix3x2<float> CombinedMatrix              => this.LayoutMatrix * this.ParentContentOffset;
    private Matrix3x2<float> CompositeParentMatrix       => (this.CompositeParentElement as Layoutable)?.Matrix ?? Matrix3x2<float>.Identity;
    private Matrix3x2<float> ParentContentOffset         => Matrix3x2<float>.Translated((this.CompositeParentElement?.ContentOffset ?? default).ToVector2<float>().InvertedX);

    private protected virtual StencilLayer?    ContentStencilLayer { get; }
    private protected virtual Matrix3x2<float> LayoutMatrix => Matrix3x2<float>.Translated(this.Offset);

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

    internal abstract bool IsParentDependent { get; }

    public sealed override Matrix3x2<float> CachedMatrix
    {
        get
        {
            if (this.matrixCache.IsInvalid)
            {
                this.matrixCache = new(this.CombinedMatrix * this.CachedCompositeParentMatrix);
            }

            return this.matrixCache.Value;
        }
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

    public override Matrix3x2<float> Matrix => this.CombinedMatrix * this.CompositeParentMatrix;

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

        var matrix = this.Matrix;

        var size     = this.Boundings.Cast<int>();
        var position = new Point<int>((int)matrix.Translation.X, -(int)matrix.Translation.Y);

        return new(size, position);
    }
}
