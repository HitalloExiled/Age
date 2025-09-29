using Age.Graphs;
using Age.Numerics;
using Age.Rendering.Resources;

namespace Age.Scenes;

public abstract class Viewport : Spatial2D
{
    public abstract event Action? Resized;

    protected RenderContext RenderContext { get; } = new();

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

            field?.ViewportOverride = null;
            value?.ViewportOverride = this;

            if (value?.Parent == null)
            {
                this.Unseal();

                field?.Detach();

                if (value != null)
                {
                    this.AppendChild(value);
                }

                this.Seal();
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

            field?.ViewportOverride = null;
            value?.ViewportOverride = this;

            if (value?.Parent == null)
            {
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

            field = value;
        }
    }

    public Window? Window { get; internal protected set; }

    public Viewport? ParentViewport
    {
        get
        {
            for (var parent = this.Parent; parent != null; parent = parent.Parent)
            {
                if (parent is Viewport viewport)
                {
                    return viewport;
                }
            }

            return null;
        }
    }

    protected Viewport() =>
        this.Seal();

    private protected override void OnConnectedInternal()
    {
        base.OnConnectedInternal();

        this.Window = this is Window window ? window : this.ParentViewport?.Window;
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnectingInternal();

        this.Window = null;
    }
}
