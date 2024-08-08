namespace Age.Platforms.Display;

public struct MouseEvent
{
    public ushort         X;
    public ushort         Y;
    public MouseButton    Button;
    public MouseKeyStates KeyStates;
    public float          Delta;
};

public struct ContextEvent
{
    public ushort X;
    public ushort Y;
    public ushort ScreenX;
    public ushort ScreenY;
};
