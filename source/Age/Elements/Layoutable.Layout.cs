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

    internal int        BaseLine  { get; private protected set; } = -1;
    internal Size<uint> Boundings { get; private protected set; }

    internal uint           LineHeight { get; set; }
    internal Vector2<float> Offset     { get; set; }

    internal virtual bool          Hidden       { get; set; }
    internal virtual StencilLayer? StencilLayer { get; set; }

    private protected virtual Transform2D LayoutTransform => Transform2D.CreateTranslated(this.Offset);

    internal abstract bool IsParentDependent { get; }

    private protected static Element? GetStyleSource(Node? node) =>
        node is not ShadowTree shadowTree
            ? (node as Element)
            : shadowTree.InheritsHostStyle
                ? shadowTree.Host
                : null;

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.OnDisposed();
        }
    }

    protected override void OnConnected(RenderTree renderTree) =>
        this.StencilLayer = this.ComposedParentElement?.ContentStencilLayer;

    protected override void OnDisconnected(RenderTree renderTree) =>
        this.StencilLayer = null;

    private protected void RequestUpdate(bool affectsBoundings)
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

    private protected void MakeDirty() =>
        this.IsDirty = true;

    private protected void MakePristine() =>
        this.IsDirty = false;

    private protected void SetCursor(Cursor? cursor)
    {
        if (this.Tree is RenderTree renderTree)
        {
            renderTree.Window.Cursor = cursor ?? default;
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
}
