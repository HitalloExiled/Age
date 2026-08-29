using Age.Elements;

namespace Age.Scenes;

public class Scene : Renderable
{
    private readonly Empty canvasSlot  = Empty.Pool.Get();
    private readonly Empty world2DSlot = Empty.Pool.Get();
    private readonly Empty world3DSlot = Empty.Pool.Get();

    public Viewport? Viewport => this.Parent as Viewport;
    public Window?   Window   => this.Viewport?.Window;

    public Canvas? Canvas
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            ReplaceSlot(this.canvasSlot, field, value);

            field = value;
        }
    }

    public World2D? World2D
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            ReplaceSlot(this.world2DSlot, field, value);

            field = value;
        }
    }

    public World3D? World3D
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            ReplaceSlot(this.world3DSlot, field, value);

            field = value;
        }
    }

    public override string NodeName => nameof(Scenes.Scene);

    public Scene() => this.AppendChildren([this.world3DSlot, this.world2DSlot, this.canvasSlot]);

    private protected override void OnDisposedInternal()
    {
        base.OnDisposedInternal();

        Empty.Pool.Return(this.canvasSlot);
        Empty.Pool.Return(this.world2DSlot);
        Empty.Pool.Return(this.world3DSlot);
    }
}
