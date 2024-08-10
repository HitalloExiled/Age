using Age.Platforms.Display;

namespace Age.Elements;

public struct MouseEvent
{
    public ushort         X;
    public ushort         Y;
    public MouseButton    Button;
    public MouseKeyStates KeyStates;
    public float          Delta;
    public Element        Target;
}
