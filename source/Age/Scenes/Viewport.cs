using Age.Graphs;
using Age.Numerics;
using Age.Rendering.Resources;
using System.Diagnostics.CodeAnalysis;

namespace Age.Scenes;

public abstract class Viewport : Spatial2D
{

    public abstract event Action? Resized;

    [AllowNull]
    internal RenderContext RenderContext { get; } = new();

    public Camera2D? Camera2D { get; set; }
    public Camera3D? Camera3D { get; set; }

    public abstract RenderTarget RenderTarget { get; }
    public abstract Size<uint>   Size         { get; set; }
    public abstract Texture2D    Texture      { get; }

    public Scene2D? Scene2D
    {
        get => field;
        set
        {
            if (field == value)
            {
                return;
            }

            if (value?.Parent == null)
            {
                this.RenderContext.ClearOverride2D();

                this.Unseal();

                field?.Detach();

                if (value != null)
                {
                    this.AppendChild(value);
                }

                this.Seal();
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
        get => field;
        set
        {
            if (field == value)
            {
                return;
            }

            if (value?.Parent == null)
            {
                this.RenderContext.ClearOverride3D();

                this.Unseal();

                field?.Detach();

                if (value != null)
                {
                    if (this.Scene2D != null)
                    {
                        this.InsertBefore(this.Scene2D, value);
                    }
                    else
                    {
                        this.AppendChild(value);
                    }
                }

                this.Seal();
            }
            else if (this.IsConnected)
            {
                this.RenderContext.Override3D(value.Viewport!.RenderContext);
            }

            field = value;
        }
    }

    public Window? Window { get; internal protected set; }

    protected Viewport() =>
        this.Seal();

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

        this.Window = this is Window window ? window : this.Scene!.Viewport!.Window;
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnectingInternal();

        this.RenderContext.ClearOverride2D();
        this.RenderContext.ClearOverride3D();

        this.Window = null;
    }
}
