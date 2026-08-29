using Age.Core.Extensions;
using Age.Graphs;
using Age.Numerics;
using Age.Rendering.Resources;
using System.Diagnostics;

namespace Age.Scenes;

[Flags]
public enum SceneFilter
{
    None    = 0,
    World2D = 1 << 0,
    World3D = 1 << 1,
    All     = World2D | World3D
}

public abstract class Viewport : Renderable
{
    public abstract event Action? Resized;

    internal RenderContext RenderContext { get; } = new();

    private readonly Empty sceneSlot = Empty.Pool.Get();

    public Camera2D? Camera2D { get; set; }
    public Camera3D? Camera3D { get; set; }

    public abstract Size<uint>   Size         { get; set; }
    public abstract RenderGraph  RenderGraph  { get; }
    public abstract RenderTarget RenderTarget { get; }
    public abstract Texture2D    Texture      { get; }

    public SceneFilter Filter
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            if (this.IsConnected)
            {
                this.BindScene();
            }
        }
    } = SceneFilter.All;

    public new Scene? Scene
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
                this.RenderContext.ClearOverrides();

                ReplaceSlot(this.sceneSlot, field, value);
            }
            else if (this.IsConnected)
            {
                this.BindScene();
            }

            field = value;
        }
    }

    public Window? Window { get; private set; }

    public abstract bool IsDirty { get; }

    protected Viewport()
    {
        this.AppendChild(this.sceneSlot);

        this.Seal();
    }

    private void BindScene()
    {
        if (this.Scene == null || this.Scene.Parent == this)
        {
            return;
        }

        this.RenderContext.ClearOverrides();

        if (this.Filter.HasFlags(SceneFilter.World2D))
        {
            this.RenderContext.Override2D(this.Scene.Viewport!.RenderContext);
        }

        if (this.Filter.HasFlags(SceneFilter.World3D))
        {
            this.RenderContext.Override3D(this.Scene.Viewport!.RenderContext);
        }
    }

    private protected override void OnConnectedInternal()
    {
        base.OnConnectedInternal();

        if (this is Window window)
        {
            this.Window = window;
        }
        else
        {
            Debug.Assert(base.Scene != null);

            this.Window = base.Scene.Window!;

            this.Window.RenderTree.AddViewport(this);

            this.BindScene();
        }
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnectingInternal();

        this.RenderContext.ClearOverride2D();
        this.RenderContext.ClearOverride3D();

        if (this.Window != this)
        {
            this.Window!.RenderTree.RemoveViewport(this);
        }

        this.Window = null;
    }

    private protected override void OnDisposedInternal()
    {
        base.OnDisposedInternal();

        Empty.Pool.Return(this.sceneSlot);
    }
}
