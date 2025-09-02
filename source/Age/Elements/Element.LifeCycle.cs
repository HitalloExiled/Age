using Age.Commands;
using Age.Scene;
using Age.Storage;

namespace Age.Elements;

public abstract partial class Element
{
    private static void DisposeLayoutCommandImage(RectCommand command)
    {
        if (!command.TextureMap.IsDefault)
        {
            TextureStorage.Singleton.Release(command.TextureMap.Texture);
        }

        if (command.StencilLayer != null)
        {
            command.StencilLayer.Dispose();
            command.StencilLayer.Detach();
        }
    }

    protected override void OnChildAppended(Node child)
    {
        if (this.ShadowTree == null && child is Layoutable layoutable)
        {
            this.HandleLayoutableAppended(layoutable);
        }
    }

    protected override void OnChildRemoved(Node child)
    {
        if (this.ShadowTree == null && child is Layoutable layoutable)
        {
            this.HandleLayoutableRemoved(layoutable);
        }
    }

    protected override void OnConnected(NodeTree tree)
    {
        base.OnConnected(tree);

        this.ShadowTree?.Tree = tree;
    }

    protected override void OnConnected(RenderTree renderTree)
    {
        base.OnConnected(renderTree);

        if (this.events.ContainsKey(EventProperty.Input))
        {
            renderTree.Window.Input += this.OnInput;
        }

        if (this.events.ContainsKey(EventProperty.KeyDown))
        {
            renderTree.Window.KeyDown += this.OnKeyDown;
        }

        if (this.events.ContainsKey(EventProperty.KeyUp))
        {
            renderTree.Window.KeyUp += this.OnKeyUp;
        }

        if (this.events.ContainsKey(EventProperty.MouseWheel))
        {
            renderTree.Window.MouseWheel += this.OnMouseWheel;
        }

        if (!renderTree.IsDirty && !this.Hidden)
        {
            renderTree.MakeDirty();
        }

        this.Canvas = this.ComposedParentElement?.Canvas ?? this.Parent as Canvas;

        this.ComputeStyle(default);

        GetStyleSource(this.Parent)?.StyleChanged += this.OnParentStyleChanged;
    }

    protected override void OnDisconnected(NodeTree tree)
    {
        base.OnDisconnected(tree);

        this.ShadowTree?.Tree = null;
    }

    protected override void OnDisconnected(RenderTree renderTree)
    {
        base.OnDisconnected(renderTree);

        this.Canvas = null;

        renderTree.Window.Input      -= this.OnInput;
        renderTree.Window.KeyDown    -= this.OnKeyDown;
        renderTree.Window.KeyUp      -= this.OnKeyUp;
        renderTree.Window.MouseWheel -= this.OnMouseWheel;

        if (!renderTree.IsDirty && !this.Hidden)
        {
            renderTree.MakeDirty();
        }
    }

    protected override void OnDisposed()
    {
        this.ownStencilLayer?.Dispose();

        if (this.TryGetLayoutCommandImage(out var layoutCommandImage))
        {
            DisposeLayoutCommandImage(layoutCommandImage);
        }

        foreach (var item in this.Commands)
        {
            CommandPool.RectCommand.Return((RectCommand)item);
        }

        this.Commands.Clear();

        this.ShadowTree?.Dispose();

        stylePool.Return(this.ComputedStyle);
    }

    protected override void OnIndexed()
    {
        var zIndex = this.ComputedStyle.ZIndex ?? this.ComposedParentElement?.ComputedStyle.ZIndex ?? 0;

        if (this.TryGetLayoutCommandBox(out var boxCommand))
        {
            boxCommand.ObjectId = this.Boundings.Area > 0 ? (uint)(this.Index + 1) : 0;
            boxCommand.ZIndex   = zIndex;
        }

        if (this.TryGetLayoutCommandScrollX(out var scrollXCommand))
        {
            scrollXCommand.ObjectId = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollX);
            scrollXCommand.ZIndex   = zIndex;
        }

        if (this.TryGetLayoutCommandScrollY(out var scrollYCommand))
        {
            scrollYCommand.ObjectId = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollY);
            scrollYCommand.ZIndex   = zIndex;
        }
    }

}
