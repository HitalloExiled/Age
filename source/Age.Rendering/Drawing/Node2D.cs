using Age.Numerics;
using Age.Rendering.Commands;

namespace Age.Rendering.Drawing;

public abstract class Node2D : Node
{
    private Transform2D    localTransform = new();
    private Vector2<float> pivot;

    private Transform2D ParentTransform => (this.Parent as Node2D)?.Transform ?? new();

    internal List<DrawCommand> Commands { get; set; } = [];

    public virtual Transform2D LocalTransform
    {
        get => this.localTransform;
        set => this.Set(ref this.localTransform, value);
    }

    public virtual Vector2<float> Pivot
    {
        get => this.pivot;
        set => this.Set(ref this.pivot, value);
    }

    public virtual Transform2D Transform
    {
        get => this.ParentTransform
            * Transform2D.Translated(this.Pivot)
            * this.LocalTransform
            * Transform2D.Translated(-this.Pivot);
        set => this.LocalTransform = value * this.ParentTransform.Inverse();
    }

    protected void Set<T>(ref T field, in T value)
    {
        if (!Equals(field, value))
        {
            field = value;
            this.OnTransformChanged();
        }
    }

    protected virtual void OnTransformChanged()
    { }
}
