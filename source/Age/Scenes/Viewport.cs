using Age.Graphs;
using Age.Numerics;
using Age.Rendering.Resources;

namespace Age.Scenes;

public abstract class Viewport : Renderable
{
    public abstract event Action? Resized;

    private readonly Empty scene2DSlot = new();
    private readonly Empty scene3DSlot = new();
    private readonly Empty uiSceneSlot = new();

    internal RenderContext RenderContext { get; } = new();

    public Camera2D? Camera2D { get; set; }
    public Camera3D? Camera3D { get; set; }

    public abstract RenderTarget RenderTarget { get; }
    public abstract Size<uint>   Size         { get; set; }
    public abstract Texture2D    Texture      { get; }

    public Scene2D? Scene2D
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            if (value?.Parent == null)
            {
                this.RenderContext.ClearOverride2D();

                ReplaceSlot(this.scene2DSlot, field, value);
            }
            else if (this.IsConnected)
            {
                this.RenderContext.Override2D(value.Viewport!.RenderContext);
            }

            field = value;
        }
    }

    public Scene3D? Scene3D
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            if (value?.Parent == null)
            {
                this.RenderContext.ClearOverride3D();

                ReplaceSlot(this.scene3DSlot, field, value);
            }
            else if (this.IsConnected)
            {
                this.RenderContext.Override3D(value.Viewport!.RenderContext);
            }

            field = value;
        }
    }

    public UIScene? UIScene
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            ReplaceSlot(this.uiSceneSlot, field, value);

            field = value;
        }
    }

    public Window? Window { get; internal protected set; }

    protected Viewport()
    {
        this.Children =
        [
            this.scene3DSlot,
            this.scene2DSlot,
            this.uiSceneSlot,
        ];

        this.Seal();
    }

    private protected override void OnConnectedInternal()
    {
        base.OnConnectedInternal();

        if (this.Scene2D != null && this.Scene2D.Parent != this)
        {
            this.RenderContext.Override2D(this.Scene2D!.Viewport!.RenderContext);
        }

        if (this.Scene3D != null && this.Scene3D.Parent != this)
        {
            this.RenderContext.Override3D(this.Scene3D!.Viewport!.RenderContext);
        }

        this.Window = this as Window ?? this.Scene!.Viewport!.Window;

        if (this is Window window)
        {
            this.Window = window;
        }
        else
        {
            this.Window = this.Scene!.Viewport!.Window;

            this.Window?.Tree.AddViewport(this);
        }
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnectingInternal();

        this.RenderContext.ClearOverride2D();
        this.RenderContext.ClearOverride3D();

        if (this.Window != this)
        {
            this.Window!.Tree.RemoveViewport(this);
        }

        this.Window = null;
    }
}
