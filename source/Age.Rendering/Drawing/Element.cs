using Age.Numerics;
using Age.Rendering.Commands;

namespace Age.Rendering.Drawing;

public class Element : Node
{
    public   Rect<int>         Bounds   { get; internal set; }
    internal List<DrawCommand> Commands { get; set; } = [];

    public Style Style { get; set; } = new();
}
