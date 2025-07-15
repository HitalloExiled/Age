using System.Runtime.CompilerServices;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Scene;

namespace Age.Elements;

public abstract partial class Layoutable
{
    internal const ushort DEFAULT_FONT_SIZE   = 16;
    internal const string DEFAULT_FONT_FAMILY = "Segoi UI";

    internal static bool IsHoveringText  { get; set; }
    internal static bool IsSelectingText { get; set; }

    private protected virtual StencilLayer? ContentStencilLayer { get; }

    internal bool IsDirty { get; private set; }

    internal protected int        BaseLine  { get; protected set; } = -1;
    internal protected Size<uint> Boundings { get; protected set; }

    internal uint LineHeight { get; set; }

    internal virtual bool          Hidden       { get; set; }
    internal virtual StencilLayer? StencilLayer { get; set; }

    private protected virtual Transform2D LayoutTransform => Transform2D.CreateTranslated(this.Offset);

    internal Vector2<float> Offset { get; set; }

    internal abstract bool       IsParentDependent { get; }

    private protected static Styleable? GetStyleSource(Node? node) =>
        node is not ShadowTree shadowTree
            ? (node as Styleable)
            : shadowTree.InheritsHostStyle
                ? shadowTree.Host
                : null;

    protected void SetCursor(Cursor? cursor)
    {
        if (this.Tree is RenderTree renderTree)
        {
            renderTree.Window.Cursor = cursor ?? default;
        }
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.OnDisposed();
        }
    }

    internal void RequestUpdate(bool affectsBoundings)
    {
        for (var current = this; ; current = current.ComposedParentElement!)
        {
            if (current.IsDirty || current.Hidden)
            {
                return;
            }

            current.MakeDirty();

            var stopPropagation = !current.IsParentDependent && !affectsBoundings || current.ComposedParentElement == null;

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

    protected override void OnConnected(RenderTree renderTree) =>
        this.StencilLayer = this.ComposedParentElement?.ContentStencilLayer;

    protected override void OnDisconnected(RenderTree renderTree) =>
        this.StencilLayer = null;

    internal void MakeDirty() =>
        this.IsDirty = true;

    internal void MakePristine() =>
        this.IsDirty = false;

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
}
