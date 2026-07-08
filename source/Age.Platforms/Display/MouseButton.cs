namespace Age.Platforms.Display;

[Flags]
public enum MouseButton : ushort
{
    None = 0,

    Left       = 1 << 0,
    Right      = 1 << 1,
    Middle     = 1 << 2,
    MbXbutton1 = 1 << 3,
    MbXbutton2 = 1 << 4,
    WheelDown  = 1 << 5,
    WheelUp    = 1 << 6,
    WheelLeft  = 1 << 7,
    WheelRight = 1 << 8,
}
