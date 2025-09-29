namespace Age.Scene;

public abstract class Scene : Renderable
{
    public event Action<Viewport?, Viewport?>? ViewportChanged;

    internal Viewport? ViewportOverride
    {
        get;
        set
        {
            if (field != value)
            {
                var old = field;

                field = value;

                this.ViewportChanged?.Invoke(field, old);
            }
        }
    }

    public Viewport? Viewport
    {
        get => this.ViewportOverride ?? field;
        internal protected set;
    }
}
