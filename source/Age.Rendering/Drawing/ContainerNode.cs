using Age.Numerics;

namespace Age.Rendering.Drawing;

public abstract class ContainerNode : Node2D
{
    private Size<uint> size;
    private Transform2D localTransform = new();

    internal float Baseline { get; set; } = 1;

    public Size<uint> ContentSize { get; internal set; }

    public override Transform2D LocalTransform
    {
        get => this.localTransform;
        set => this.Set(ref this.localTransform, value);
    }

    public Size<uint> Size
    {
        get => this.size;
        internal set => this.Set(ref this.size, value);
    }

    private void Set<T>(ref T field, in T value)
    {
        if (!Equals(field, value))
        {
            field = value;
            this.OnBoundsChanged();
        }
    }

    protected virtual void OnBoundsChanged()
    { }
}
