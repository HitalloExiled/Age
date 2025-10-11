using Age.Commands;
using Age.Scenes;
using Age.Storage;

namespace Age.Elements;

public abstract partial class Element
{
    private void DisposeLayoutCommandImage(RectCommand command)
    {
        if (!command.TextureMap.IsDefault)
        {
            TextureStorage.Singleton.Release(command.TextureMap.Texture);
        }

        if (command.StencilLayer?.Owner == this)
        {
            command.StencilLayer.Detach();
        }
    }

    private protected override void OnAttachedInternal()
    {
        base.OnAttachedInternal();

        GetStyleSource(this.Parent)?.StyleChanged += this.OnParentStyleChanged;
    }

    private protected override void OnChildAttachedInternal(Node node)
    {
        base.OnChildAttachedInternal(node);

        if (this.ShadowTree == null && node is Layoutable layoutable)
        {
            this.HandleLayoutableAppended(layoutable);
        }
    }

    private protected override void OnChildDetachingInternal(Node node)
    {
        base.OnChildDetachingInternal(node);

        if (this.ShadowTree == null && node is Layoutable layoutable)
        {
            this.HandleLayoutableRemoved(layoutable);
        }
    }

    private protected override void OnConnectedInternal()
    {
        base.OnConnectedInternal();

        this.ShadowTree?.Connect();

        if (this.Scene?.Viewport?.Window is Window window)
        {
            if (this.events.ContainsKey(EventProperty.Input))
            {
                window.Input += this.OnInput;
            }

            if (this.events.ContainsKey(EventProperty.KeyDown))
            {
                window.KeyDown += this.OnKeyDown;
            }

            if (this.events.ContainsKey(EventProperty.KeyUp))
            {
                window.KeyUp += this.OnKeyUp;
            }

            if (this.events.ContainsKey(EventProperty.MouseWheel))
            {
                window.MouseWheel += this.OnMouseWheel;
            }

            if (!window.Tree.IsDirty && !this.Hidden)
            {
                window.Tree.MakeDirty();
            }
        }

        this.Canvas = this.ComposedParentElement?.Canvas ?? this.Parent as Canvas;

        this.ComputeStyle(default);
    }

    private protected override void OnDetachingInternal()
    {
        base.OnDetachingInternal();

        GetStyleSource(this.Parent)?.StyleChanged -= this.OnParentStyleChanged;
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnectingInternal();

        this.ShadowTree?.Disconnect();

        this.Canvas = null;

        if (this.Scene?.Viewport?.Window is Window window)
        {
            window.Input      -= this.OnInput;
            window.KeyDown    -= this.OnKeyDown;
            window.KeyUp      -= this.OnKeyUp;
            window.MouseWheel -= this.OnMouseWheel;

            if (!window.Tree.IsDirty && !this.Hidden)
            {
                window.Tree.MakeDirty();
            }
        }
    }

    private protected override void OnDisposedInternal()
    {
        if (this.TryGetLayoutCommandImage(out var layoutCommandImage))
        {
            this.DisposeLayoutCommandImage(layoutCommandImage);
        }

        foreach (var item in this.Commands)
        {
            CommandPool.RectCommand.Return((RectCommand)item);
        }

        this.ClearCommands();

        this.ShadowTree?.Dispose();

        stylePool.Return(this.ComputedStyle);
    }

    private protected override void OnIndexChanged()
    {
        var zIndex = this.ComputedStyle.ZIndex ?? this.ComposedParentElement?.ComputedStyle.ZIndex ?? 0;

        if (this.TryGetLayoutCommandBox(out var boxCommand))
        {
            boxCommand.Metadata = this.Boundings.Area > 0 ? (uint)(this.Index + 1) : 0;
            boxCommand.ZIndex   = zIndex;
        }

        if (this.TryGetLayoutCommandScrollBarX(out var scrollXCommand))
        {
            scrollXCommand.Metadata = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollBarX);
            scrollXCommand.ZIndex   = zIndex;
        }

        if (this.TryGetLayoutCommandScrollBarY(out var scrollYCommand))
        {
            scrollYCommand.Metadata = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollBarY);
            scrollYCommand.ZIndex   = zIndex;
        }
    }
}
