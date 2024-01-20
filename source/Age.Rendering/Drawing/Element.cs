using Age.Rendering.Commands;

namespace Age.Rendering.Drawing;

public class Element : Node
{
    internal List<DrawCommand> Commands { get; set; } = [];

    public Style Style { get; set; } = new();
}
