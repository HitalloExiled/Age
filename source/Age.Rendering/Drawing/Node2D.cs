using Age.Numerics;
using Age.Rendering.Commands;

namespace Age.Rendering.Drawing;

public abstract class Node2D : Node
{
    internal List<DrawCommand> Commands { get; set; } = [];

    public virtual Transform2D Transform
    {
        get => this.LocalTransform * ((this.Parent as Node2D)?.Transform ?? new());
        set => this.LocalTransform = value * ((this.Parent as Node2D)?.Transform ?? new()).Inverse();
    }

    public virtual Transform2D LocalTransform { get; set; }
}
