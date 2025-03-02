using Age.Commands;
using Age.Numerics;

namespace Age.Scene;

public sealed partial class RenderTree
{

    public struct Command3DEntry(Command command, Matrix4x4<float> transform)
    {
        public Command Command = command;
        public Matrix4x4<float> Transform = transform;
    }
}
