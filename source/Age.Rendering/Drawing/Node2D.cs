using Age.Numerics;
using Age.Rendering.Commands;

namespace Age.Rendering.Drawing;

public abstract class Node2D : Node
{
    private Size<float> size;
    private Transform2D localTransform = new();

    internal List<DrawCommand> Commands { get; set; } = [];

    public float BaseLine { get; set; } = 1;

    public Size<float> Size
    {
        get => this.size;
        internal set
        {
            var hasChanged = this.size != value;

            this.size = value;

            if (hasChanged)
            {
                this.OnBoundsChanged();
            }
        }
    }

    public virtual Transform2D Transform
    {
        get => this.localTransform * ((this.Parent as Node2D)?.Transform ?? new());
        set => this.LocalTransform = value * ((this.Parent as Node2D)?.Transform ?? new()).Inverse();
    }

    public virtual Transform2D LocalTransform
    {
        get => this.localTransform;
        set
        {
            var hasChanged = this.localTransform != value;

            this.localTransform = value;

            if (hasChanged)
            {
                this.OnBoundsChanged();
            }
        }
    }

    protected virtual void OnBoundsChanged()
    { }

    public override string ToString() =>
        $"<{this.NodeName} name='{this.Name}' size='w: {this.Size.Width}, h: {this.Size.Width}' position='x: {this.Transform.Position.X}, y: {this.Transform.Position.Y}' />";
}
