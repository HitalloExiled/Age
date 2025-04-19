using Age.Commands;
using Age.Numerics;

namespace Age.Scene;

public sealed partial class RenderTree
{
    public struct Command2DEntry(Command command, Transform2D transform)
    {
        public Command     Command   = command;
        public Transform2D Transform = transform;
    }
}
