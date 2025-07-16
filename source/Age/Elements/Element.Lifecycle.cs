using Age.Scene;

namespace Age.Elements;

public abstract partial class Element
{
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

        if (this.events.ContainsKey(EventProperty.Scrolled))
        {
            renderTree.Window.MouseWheel += this.OnScroll;
        }

        if (!renderTree.IsDirty && !this.Hidden)
        {
            renderTree.MakeDirty();
        }

        this.Canvas = this.ComposedParentElement?.Canvas ?? this.Parent as Canvas;

        this.ComputeStyle(default);

        if (this.ownStencilLayer != null)
        {
            this.StencilLayer?.AppendChild(this.ownStencilLayer);
        }

        GetStyleSource(this.Parent)?.StyleChanged += this.OnParentStyleChanged;
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
        renderTree.Window.MouseWheel -= this.OnScroll;

        if (!renderTree.IsDirty && !this.Hidden)
        {
            renderTree.MakeDirty();
        }
    }

    protected override void OnRemoved(Node parent)
    {
        base.OnRemoved(parent);

        GetStyleSource(parent)?.StyleChanged -= this.OnParentStyleChanged;
    }
}
