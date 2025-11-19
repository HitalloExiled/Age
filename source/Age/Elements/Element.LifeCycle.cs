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

        this.CompositeParentElement?.StyleChanged += this.OnParentStyleChanged;
    }

    private protected override void OnChildAttachedInternal(Node child)
    {
        base.OnChildAttachedInternal(child);

        if (child is Layoutable layoutable)
        {
            this.HandleLayoutableAppended(layoutable);
        }
    }

    private protected override void OnChildDetachingInternal(Node child)
    {
        base.OnChildDetachingInternal(child);

        if (child is Layoutable layoutable)
        {
            this.HandleLayoutableRemoved(layoutable);
        }
    }

    private protected override void OnConnectedInternal()
    {
        base.OnConnectedInternal();

        if (this.Scene?.Window is Window window)
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

            if (!window.Tree.IsDirty && this.Visible)
            {
                window.Tree.MakeDirty();
            }
        }

        this.ComputeStyle(default);
    }

    private protected override void OnDetachingInternal()
    {
        base.OnDetachingInternal();

        this.CompositeParentElement?.StyleChanged -= this.OnParentStyleChanged;
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnectingInternal();

        if (this.Scene?.Viewport?.Window is Window window)
        {
            window.Input      -= this.OnInput;
            window.KeyDown    -= this.OnKeyDown;
            window.KeyUp      -= this.OnKeyUp;
            window.MouseWheel -= this.OnMouseWheel;

            if (!window.Tree.IsDirty && this.Visible)
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

        stylePool.Return(this.ComputedStyle);
    }

    private protected override void OnIndexChangedInternal()
    {
        var zIndex = this.ComputedStyle.ZIndex ?? this.CompositeParentElement?.ComputedStyle.ZIndex ?? 0;

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
