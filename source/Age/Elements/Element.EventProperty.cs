namespace Age.Elements;

public abstract partial class Element
{
    private enum EventProperty
    {
        None          = 0,
        Activated     = 1 << 0,
        Blured        = 1 << 1,
        Clicked       = 1 << 2,
        Context       = 1 << 3,
        Deactivated   = 1 << 4,
        DoubleClicked = 1 << 5,
        Focused       = 1 << 6,
        Input         = 1 << 7,
        KeyDown       = 1 << 8,
        KeyUp         = 1 << 9,
        MouseDown     = 1 << 10,
        MouseEnter    = 1 << 11,
        MouseLeave    = 1 << 12,
        MouseMoved    = 1 << 13,
        MouseOut      = 1 << 14,
        MouseOver     = 1 << 15,
        MouseRelease  = 1 << 16,
        MouseUp       = 1 << 17,
        MouseWheel    = 1 << 18,
    }
}
