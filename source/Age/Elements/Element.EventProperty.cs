namespace Age.Elements;

public abstract partial class Element
{
    private enum EventProperty
    {
        None = 0,
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
        MouseMoved    = 1 << 11,
        MouseOut      = 1 << 12,
        MouseOver     = 1 << 13,
        MouseUp       = 1 << 14,
        MouseWheel      = 1 << 15,
    }
}
