using Age.Rendering.Enums;

namespace Age.Rendering.Commands;

public abstract record DrawCommand(DrawCommandType type)
{
    public DrawCommandType Type { get; } = type;
    public DrawCommand?    Next { get ; set; }
}
