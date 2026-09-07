namespace Age.Scenes;

public class Scene : Renderable
{
    private readonly Empty canvasSlot  = new();
    private readonly Empty world2DSlot = new();
    private readonly Empty world3DSlot = new();

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

    public Scene[] Scenes
    {
        get
        {
            var scenes = new List<Scene>();

            foreach (var item in this)
            {
                if (item is Scene scene)
                {
                    scenes.Add(scene);
                }
            }

            return [.. scenes];
        }
        set => this.AppendChildren(value);
    }

    public override string NodeName => nameof(Age.Scenes.Scene);

    public Scene()
    {
        var composition = new Composition();

        composition.AppendChildren([this.world3DSlot, this.world2DSlot, this.canvasSlot]);

        this.AttachShadowRoot(composition);
    }
}
