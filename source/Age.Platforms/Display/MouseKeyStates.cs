namespace Age.Platforms.Display;

[Flags]
public enum MouseKeyStates
{
    None         = 0,
    LeftButton   = 0x0001,
    RightButton  = 0x0002,
    Shift        = 0x0004,
    Control      = 0x0008,
    MiddleButton = 0x0010,
    Xbutton1     = 0x0020,
    Xbutton2     = 0x0040,
}
