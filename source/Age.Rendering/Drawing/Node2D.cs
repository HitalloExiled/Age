using Age.Numerics;
using Age.Rendering.Commands;

namespace Age.Rendering.Drawing;

public abstract class Node2D : Node
{
    private Size<float> size;
    private Transform2D transform = new();

    internal List<DrawCommand> Commands { get; set; } = [];

    public Size<float> Size
    {
        get => this.size;
        set
        {
            var hasChanged = this.size != value;

            this.size = value;

            if (hasChanged)
            {
                this.OnBoundsChanged();
            }
        }
    }

    public Transform2D Transform
    {
        get => this.transform;
        set
        {
            var hasChanged = this.transform != value;

            this.transform = value;

            if (hasChanged)
            {
                this.OnBoundsChanged();
            }
        }
    }

    protected virtual void OnBoundsChanged()
    { }

    public override string ToString() =>
        $"<{this.NodeName} name='{this.Name}' size='w: {this.Size.Width}, h: {this.Size.Width}' position='x: {this.Transform.Position.X}, y: {this.Transform.Position.Y}' >";
}
