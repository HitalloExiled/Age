using System.Diagnostics;
using Age.Commands;
using Age.Scene;
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

    protected override void OnConnected()
    {
        base.OnConnected();

        this.ShadowTree?.Connect();

        if (this.Window != null)
        {
            if (this.events.ContainsKey(EventProperty.Input))
            {
                this.Window.Input += this.OnInput;
            }

            if (this.events.ContainsKey(EventProperty.KeyDown))
            {
                this.Window.KeyDown += this.OnKeyDown;
            }

            if (this.events.ContainsKey(EventProperty.KeyUp))
            {
                this.Window.KeyUp += this.OnKeyUp;
            }

            if (this.events.ContainsKey(EventProperty.MouseWheel))
            {
                this.Window.MouseWheel += this.OnMouseWheel;
            }

            if (!this.Window.Tree.IsDirty && !this.Hidden)
            {
                this.Window.Tree.MakeDirty();
            }
        }

        this.Canvas = this.ComposedParentElement?.Canvas ?? this.Parent as Canvas;

        this.ComputeStyle(default);

        GetStyleSource(this.Parent)?.StyleChanged += this.OnParentStyleChanged;
    }

    protected override void OnDisconnected()
    {
        base.OnDisconnected();

        this.ShadowTree?.Disconnect();

        this.Canvas = null;

        if (this.Window != null)
        {
            this.Window.Input      -= this.OnInput;
            this.Window.KeyDown    -= this.OnKeyDown;
            this.Window.KeyUp      -= this.OnKeyUp;
            this.Window.MouseWheel -= this.OnMouseWheel;

            if (!this.Window.Tree.IsDirty && !this.Hidden)
            {
                this.Window.Tree.MakeDirty();
            }
        }
    }

    protected override void OnDisposed()
    {
        if (this.TryGetLayoutCommandImage(out var layoutCommandImage))
        {
            this.DisposeLayoutCommandImage(layoutCommandImage);
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

        if (this.TryGetLayoutCommandScrollBarX(out var scrollXCommand))
        {
            scrollXCommand.ObjectId = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollBarX);
            scrollXCommand.ZIndex   = zIndex;
        }

        if (this.TryGetLayoutCommandScrollBarY(out var scrollYCommand))
        {
            scrollYCommand.ObjectId = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollBarY);
            scrollYCommand.ZIndex   = zIndex;
        }
    }

}
